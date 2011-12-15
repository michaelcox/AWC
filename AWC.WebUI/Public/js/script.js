(function() {
  var DEFAULT_SEARCH_VALUE, addAutoClear, getDistinctItems, getId, getRequestedItemTemplate, moveClient, scheduleClient, searchClients;

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

  searchClients = function(searchTerm, add) {
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

  getDistinctItems = function(searchTerm, add) {
    return $.getJSON("/search/distinctitems?q=" + searchTerm, function(data) {
      var r, result;
      r = (function() {
        var _i, _len, _results;
        _results = [];
        for (_i = 0, _len = data.length; _i < _len; _i++) {
          result = data[_i];
          _results.push({
            label: result,
            value: result
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
        return $('#requesteditems').append(data);
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

  jQuery(function($) {
    addAutoClear($('#search'), DEFAULT_SEARCH_VALUE);
    $('.phone').mask('(999) 999-9999');
    $('#flashmessage').fadeIn(1000).delay(4000).fadeOut(1000);
    $('#search').autocomplete({
      source: function(req, add) {
        return searchClients(req.term, add);
      },
      select: function(event, ui) {
        return $('#searchClientId').val(ui.item.id);
      }
    });
    $('td.item_name > input').live('keyup.autocomplete', function() {
      return $(this).autocomplete({
        source: function(req, add) {
          return getDistinctItems(req.term, add);
        }
      });
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
  });

}).call(this);
