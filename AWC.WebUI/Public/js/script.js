/* jQuery DOMdocument.ready */
$(document).ready(function () {

    // Search
    clearSearchFields();

    //attach autocomplete
    $("#search").autocomplete({

        //define callback to format results
        source: function (req, add) {

            var q = req.term;

            //pass request to server
            $.getJSON("/search?q=" + q, function (data) {

                //create array for response objects
                var suggestions = [];

                //process response
                $.each(data, function (i, val) {
                    var label = val.FirstName + " " + val.LastName;
                    label += " - " + "(" + val.PrimaryPhoneNumber.substring(0, 3) + ")";
                    label += " " + val.PrimaryPhoneNumber.substring(3, 6) + "-";
                    label += val.PrimaryPhoneNumber.substring(6);
                    suggestions.push({ "label": label, "value": label, "id": val.ClientId });
                });

                //pass array to callback
                add(suggestions);
            });
        },
        select: function (event, ui) { $('#searchClientId').val(ui.item.id); }
    });

    // Phone Number validation
    $("#PrimaryPhoneNumber").mask("(999) 999-9999");
    $("#SecondaryPhoneNumber").mask("(999) 999-9999");

    // Flash Message
    $('#flashmessage').fadeIn(1000).delay(4000).fadeOut(1000);

    // Calendar
    $('.calendar-default #calendar').fullCalendar({
        eventSources: [
                {
                    url: 'http://www.google.com/calendar/feeds/usa__en%40holiday.calendar.google.com/public/basic',
                    type: 'GET',
                    color: '#F0E68C',
                    textColor: '#666'
                },
                {
                    url: '/Schedule/CalendarEvents',
                    type: 'GET',
                    data: { id: getId() },
                    dataType: 'json'
                }
            ],
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek'
        },
        disableResizing: true,
        editable: true,
        eventDrop: function (event, dayDelta, minuteDelta, allDay, revertFunc) {

            if (!confirm("Are you sure you want to move this appointment?")) {
                revertFunc();
            } else {
                $.ajax({
                    url: '/Schedule/Edit',
                    cache: false,
                    dataType: 'json',
                    type: 'POST',
                    data: { id: event.id, dayDelta: dayDelta, minDelta: minuteDelta }
                });
            }
        },
        defaultEventMinutes: 30,
        firstDay: 1,
        defaultView: 'agendaWeek',
        allDayDefault: false,
        ignoreTimezone: false,
        allDayText: 'Holidays', // Figuring all day events are imported from Google only
        minTime: 5,
        maxTime: 24
    });

    $('#waitlist-clients .waitlist-client').each(function () {

        // it doesn't need to have a start or end
        var eventObject = {
            title: $.trim($(this).text()) // use the element's text as the event title
        };

        // store the Event Object in the DOM element so we can get to it later
        $(this).data('eventObject', eventObject);

        // make the event draggable using jQuery UI
        $(this).draggable({
            zIndex: 999,
            revert: true,      // will cause the event to go back to its
            revertDuration: 0  //  original position after the drag
        });

    });

    $('.calendar-schedule #calendar').fullCalendar({
        eventSources: [
                {
                    url: 'http://www.google.com/calendar/feeds/usa__en%40holiday.calendar.google.com/public/basic',
                    type: 'GET',
                    color: '#F0E68C',
                    textColor: '#666'
                },
                {
                    url: '/Schedule/CalendarEvents',
                    type: 'GET',
                    dataType: 'json'
                }
            ],
        header: {
            left: 'today',
            center: 'title',
            right: 'today prev,next'
        },
        droppable: true,
        drop: function (date, allDay) {
            var eventObject = $(this).data('eventObject');
            eventObject.start = date;
            eventObject.allDay = false;
            eventObject.editable = true;

            var clientId = $(this).attr('id'); // Grab the id of the div
            clientId = clientId.substring(5);  // Strip "user-" from the id

            $.ajax({
                url: "/Schedule/Create",
                type: "POST",
                data: { id: clientId, scheduledDate: date.toUTCString() },
                dataType: "json"
            });

            $('#calendar').fullCalendar('renderEvent', eventObject, true);
            $(this).remove();
        },
        disableResizing: true,
        editable: true,
        eventDrop: function (event, dayDelta, minuteDelta, allDay, revertFunc) {

            if (!confirm("Are you sure you want to move this appointment?")) {
                revertFunc();
            } else {
                $.ajax({
                    url: '/Schedule/Edit',
                    cache: false,
                    dataType: 'json',
                    type: 'POST',
                    data: { id: event.id, dayDelta: dayDelta, minDelta: minuteDelta }
                });
            }
        },
        defaultEventMinutes: 30,
        firstDay: 1,
        defaultView: 'agendaWeek',
        allDayDefault: false,
        ignoreTimezone: false,
        allDayText: 'Holidays', // Figuring all day events are imported from Google only
        minTime: 5,
        maxTime: 24
    });

    $('#calendar').fullCalendar('rerenderEvents');

    // Requested Items
    $('#add_item').click(function (e) {
        $.ajax({
            url: '/Clients/1/RequestedItemTemplate',
            cache: false,
            dataType: 'html',
            success: function (data) {
                $('#requesteditems').append(data);
            }
        });
        e.preventDefault();
    });
    $("a.deleteRow").live("click", function () {
        $(this).parents("tr:first").remove();
        return false;
    });
});
function clearSearchFields() {
    var _field = $('.search input');
    var _defaultVal = _field.val();

    // On focus - empty field
    _field.focus(function () {
        if ($(this).val() == _defaultVal)
            $(this).val('');
    });

    // On blur - refill if blank
    _field.blur(function () {
        if ($(this).val() == '')
            $(this).val(_defaultVal);
    });
}

function getId() {
    var regex = new RegExp(/\d/);
    var path = window.location.pathname;
    var id = regex.exec(path);
    if (id == null)
        return 0;
    else
        return id[0];
}