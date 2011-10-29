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

    // jQuery Calendar Plugin
    $('#calendar').fullCalendar({
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