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
                    suggestions.push(val.FirstName + " " + val.LastName + " (" + val.ClientId + ")");
                });

                //pass array to callback
                add(suggestions);
            });
        }
    });

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
                    dataType: 'json'
                }
            ],
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek'
        },
        defaultEventMinutes: 30,
        firstDay: 1,
        defaultView: 'month',
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
            $('#calendar').fullCalendar('renderEvent', eventObject, true);
            $(this).remove();
        },
        disableResizing: true,
        eventDrop: function (event, dayDelta, minuteDelta, allDay, revertFunc) {

            alert(
                        event.title + " was moved " +
                        dayDelta + " days and " +
                        minuteDelta + " minutes."
                    );

            if (allDay) {
                alert("Event is now all-day");
            } else {
                alert("Event has a time-of-day");
            }

            if (!confirm("Are you sure about this change?")) {
                revertFunc();
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