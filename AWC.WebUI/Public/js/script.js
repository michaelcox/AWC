(function() {
  var DEFAULT_SEARCH_VALUE, addAutoClear, editScheduledDateTime, getId, getRequestedItemTemplate, markAction, moveClient, properlyHighlightToDoItems, scheduleClient, searchClients, sumIncomes, updateScheduledDate;

  DEFAULT_SEARCH_VALUE = 'Client Search';

  addAutoClear = function(inputBox, defaultValue) {
    inputBox.val(defaultValue);
    inputBox.focus(function() {
      if (inputBox.val() === defaultValue) return inputBox.val('');
    });
    return inputBox.blur(function() {
      if (inputBox.val() === '') return inputBox.val(defaultValue);
    });
  };

  searchClients = function(req, add) {
    var searchTerm;
    searchTerm = req.term;
    return $.getJSON("/search?q=" + searchTerm, function(data) {
      var id, label, name, phoneNumber, r, result;
      r = (function() {
        var _i, _len, _results;
        _results = [];
        for (_i = 0, _len = data.length; _i < _len; _i++) {
          result = data[_i];
          name = result.FirstName + " " + result.LastName;
          phoneNumber = "(" + result.PrimaryPhoneNumber.substring(0, 3) + ")";
          phoneNumber += " " + result.PrimaryPhoneNumber.substring(3, 6) + "-";
          phoneNumber += result.PrimaryPhoneNumber.substring(6);
          label = name + " - " + phoneNumber;
          id = result.ClientId;
          _results.push({
            label: label,
            value: label,
            id: id
          });
        }
        return _results;
      })();
      return add(r);
    });
  };

  getRequestedItemTemplate = function() {
    return $.ajax({
      url: '/Clients/1/RequestedItemTemplate',
      cache: false,
      dataType: 'html',
      success: function(data) {
        return $('#requesteditems').find('tbody').append(data);
      }
    });
  };

  scheduleClient = function(clientId, date) {
    return $.ajax({
      url: "/Schedule/Create",
      type: "POST",
      data: {
        id: clientId,
        scheduledDate: date.toUTCString()
      },
      dataType: "json"
    });
  };

  moveClient = function(eventId, dayDelta, minDelta) {
    return $.ajax({
      url: '/Schedule/Move',
      cache: false,
      dataType: 'json',
      type: 'POST',
      data: {
        id: eventId,
        dayDelta: dayDelta,
        minDelta: minDelta
      }
    });
  };

  getId = function() {
    var id, regex, _ref;
    regex = new RegExp(/\d/);
    id = regex.exec(window.location.pathname);
    return (_ref = id != null ? id[0] : void 0) != null ? _ref : 0;
  };

  updateScheduledDate = function() {
    var ampm, day, hour, min, month, year, _ref, _ref2, _ref3;
    day = (_ref = $('#scheduledDate').datepicker('getDate')) != null ? _ref.getDate() : void 0;
    month = ((_ref2 = $('#scheduledDate').datepicker('getDate')) != null ? _ref2.getMonth() : void 0) + 1;
    year = (_ref3 = $('#scheduledDate').datepicker('getDate')) != null ? _ref3.getFullYear() : void 0;
    hour = $('#scheduledHour').val();
    min = $('#scheduledMinute').val();
    ampm = $('#scheduledAmPm').val();
    $('#ScheduledDateTime').val(month + '/' + day + '/' + year + ' ' + hour + ':' + min + ' ' + ampm);
  };

  editScheduledDateTime = function() {
    var clientId;
    clientId = $('#ClientId').val();
    return $.ajax({
      url: "/Schedule/Edit",
      type: "POST",
      data: {
        id: clientId,
        dateString: $('#ScheduledDateTime').val()
      },
      dataType: "json",
      success: function(data) {
        var checkboxId, _i, _len, _ref;
        if (data.success) {
          $('#scheduledDateFlash').html('Appointment date updated!');
          _ref = ['action_two_day', 'action_two_week'];
          for (_i = 0, _len = _ref.length; _i < _len; _i++) {
            checkboxId = _ref[_i];
            $('#' + checkboxId).removeAttr('checked').removeAttr('disabled');
          }
          properlyHighlightToDoItems();
        } else {
          $('#scheduledDateFlash').html('ERROR SAVING');
        }
        return $('#scheduledDateFlash').fadeIn(1000).delay(4000).fadeOut(1000);
      },
      error: function() {
        $('#scheduledDateFlash').html('ERROR SAVING');
        return $('#scheduledDateFlash').fadeIn(1000).delay(4000).fadeOut(1000);
      }
    });
  };

  markAction = function(action, checkboxId, labelId) {
    var apptId;
    apptId = $('#appointmentId').val();
    return $.ajax({
      url: window.location.pathname + "/" + action,
      type: "POST",
      data: {
        apptId: apptId
      },
      dataType: "json",
      success: function(data) {
        if (data.success) {
          return properlyHighlightToDoItems();
        } else {
          return $('#' + checkboxId).removeAttr('checked');
        }
      },
      error: function() {
        return $('#' + checkboxId).removeAttr('checked');
      }
    });
  };

  properlyHighlightToDoItems = function() {
    var apptDate, currentTime, daysLeft, oneDay;
    apptDate = new Date($('#ScheduledDateTime').val());
    currentTime = new Date();
    oneDay = 1000 * 60 * 60 * 24;
    daysLeft = Math.ceil((apptDate.getTime() - currentTime.getTime()) / oneDay);
    $('#todo span').each(function() {
      return $(this).attr('class', 'default');
    });
    $('#label_send_letter').attr('class', 'needed');
    if (daysLeft <= 3) $('#label_two_day').attr('class', 'needed');
    if (daysLeft <= 15) $('#label_two_week').attr('class', 'needed');
    return $('#todo input').each(function() {
      if ($(this).is(':checked')) {
        $(this).attr('disabled', 'disabled');
        return $(this).next('span').attr('class', 'done');
      }
    });
  };

  sumIncomes = function() {
    var totalMonthlyIncome;
    totalMonthlyIncome = 0;
    return $('td.monthlyIncome input').each(function(index, elem) {
      totalMonthlyIncome += parseFloat($(elem).val());
      return $('#totalIncome').text('$' + (totalMonthlyIncome * 12).formatMoney());
    });
  };

  Number.prototype.formatMoney = function(c, d, t) {
    var i, j, n, s;
    n = this;
    c = (isNaN(c = Math.abs(c)) ? 2 : c);
    d = (d === void 0 ? "." : d);
    t = (t === void 0 ? "," : t);
    s = (n < 0 ? "-" : "");
    i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "";
    j = ((j = i.length) > 3 ? j % 3 : 0);
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
  };

  jQuery(function($) {
    var day, hour, initialDate, min, month, year, _i, _len, _ref, _ref2;
    addAutoClear($('#search'), DEFAULT_SEARCH_VALUE);
    $('.phone').mask('(999) 999-9999');
    $('.success').fadeIn(1000).delay(4000).fadeOut(1000);
    $('.info').fadeIn(1000).delay(4000).fadeOut(1000);
    $('#search').autocomplete({
      source: searchClients,
      select: function(event, ui) {
        return $('#searchClientId').val(ui.item.id);
      },
      autoFocus: true
    });
    $('td.item_name > input').live('keyup.autocomplete', function() {
      return $(this).autocomplete({
        source: window.distinctItems,
        delay: 0,
        minLength: 1,
        autoFocus: true
      });
    });
    $('#PartneringOrganization').autocomplete({
      source: window.orgs,
      delay: 0,
      minLength: 1,
      autoFocus: true
    });
    $('#OtherReferrals').autocomplete({
      source: window.otherReferrals,
      delay: 0,
      minLength: 1,
      autoFocus: true
    });
    $('#add_item').click(function(e) {
      e.preventDefault();
      return getRequestedItemTemplate();
    });
    $('a.deleteRow').live('click', function(e) {
      e.preventDefault();
      return $(this).parents('tr:first').remove();
    });
    $('#calendar').fullCalendar({
      eventSources: [
        {
          url: 'http://www.google.com/calendar/feeds/usa__en%40holiday.calendar.google.com/public/basic',
          type: 'GET',
          color: '#F0E68C',
          textColor: '#666'
        }, {
          url: '/Schedule/CalendarEvents',
          type: 'GET',
          dataType: 'json',
          data: {
            id: getId()
          }
        }
      ],
      header: {
        left: 'today',
        center: 'title',
        right: 'today prev,next'
      },
      drop: function(date, allDay) {
        var clientId, eventObject;
        eventObject = $(this).data('eventObject');
        eventObject.start = date;
        eventObject.allDay = false;
        eventObject.editable = true;
        clientId = $(this).attr('id').substring(5);
        scheduleClient(clientId, date);
        $('#calendar').fullCalendar('renderEvent', eventObject, true);
        return $(this).remove();
      },
      eventDrop: function(event, dayDelta, minuteDelta, allDay, revertFunc) {
        if (confirm("Are you sure you want to move this appointment?")) {
          return moveClient(event.id, dayDelta, minuteDelta);
        } else {
          return revertFunc();
        }
      },
      disableResizing: true,
      editable: true,
      droppable: true,
      defaultEventMinutes: 30,
      firstDay: 1,
      defaultView: 'agendaWeek',
      allDayDefault: false,
      ignoreTimezone: false,
      allDayText: 'Holidays',
      minTime: 5,
      maxTime: 24
    });
    if ($('#focusDate').length) {
      _ref = $('#focusDate').val().split(','), year = _ref[0], month = _ref[1], day = _ref[2];
      month = parseInt(month);
      year = parseInt(year);
      day = parseInt(day);
      $('#calendar').fullCalendar('gotoDate', year, month, day);
    }
    $('#waitlist-clients .waitlist-client').each(function() {
      var eventObject;
      eventObject = {
        title: $.trim($(this).text())
      };
      $(this).data('eventObject', eventObject);
      return $(this).draggable({
        zIndex: 999,
        revert: true,
        revertDuration: 0
      });
    });
    $('#searchForm').submit(function() {
      if ($('#searchClientId').val() === "") {
        alert('You must select a client from the dropdown options.');
        return false;
      }
    });
    $('#scheduledDate').datepicker({
      defaultDate: new Date($('#ScheduledDateTime').val()),
      onSelect: updateScheduledDate,
      firstDay: 1
    });
    for (hour = 1; hour <= 12; hour++) {
      $('#scheduledHour').append($("<option></option>").attr('value', hour).text(hour));
    }
    _ref2 = ['00', '30'];
    for (_i = 0, _len = _ref2.length; _i < _len; _i++) {
      min = _ref2[_i];
      $('#scheduledMinute').append($("<option></option>").attr('value', min).text(min));
    }
    initialDate = new Date($('#ScheduledDateTime').val());
    $('#scheduledMinute').val(initialDate.getMinutes());
    if (initialDate.getHours() <= 12) {
      $('#scheduledHour').val(initialDate.getHours());
      $('#scheduledAmPm').val('AM');
    } else {
      $('#scheduledHour').val(initialDate.getHours() - 12);
      $('#scheduledAmPm').val('PM');
    }
    $('#scheduledHour').change(updateScheduledDate);
    $('#scheduledMinute').change(updateScheduledDate);
    $('#scheduledAmPm').change(updateScheduledDate);
    $('#updateScheduledDate').click(editScheduledDateTime);
    if ($('#todo').length) {
      properlyHighlightToDoItems();
      $('#action_send_letter').change(function() {
        return markAction('SentMailing', 'action_send_letter', 'label_send_letter');
      });
      $('#action_two_week').change(function() {
        return markAction('CompletedTwoWeekConfirmation', 'action_two_week', 'label_two_week');
      });
      $('#action_two_day').change(function() {
        return markAction('CompletedTwoDayConfirmation', 'action_two_day', 'label_two_day');
      });
    }
    $('td.monthlyIncome input').change(function() {
      return sumIncomes();
    });
    sumIncomes();
  });

}).call(this);
