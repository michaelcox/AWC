DEFAULT_SEARCH_VALUE = 'Search'

addAutoClear = (inputBox, defaultValue) ->
	inputBox.val(defaultValue)
	
	inputBox.focus ->
		inputBox.val('') if (inputBox.val() is defaultValue)

	inputBox.blur ->
		inputBox.val(defaultValue) if (inputBox.val() is '')
		
searchClients = (searchTerm) ->
	$.getJSON "/search?q=" + q, (data) ->
		for result in data
			name = result.FirstName + " " + result.LastName
			phoneNumber = "(" + val.PrimaryPhoneNumber.substring(0, 3) + ")";
			phoneNumber += " " + val.PrimaryPhoneNumber.substring(3, 6) + "-";
			phoneNumber += val.PrimaryPhoneNumber.substring(6);
			label = name + " - " + phoneNumber
			id = result.ClientId
			{label, value: label, id}
			
getDistinctItems = (searchTerm) ->
	$.getJSON "/search/distinctitems?q=" + q, (data) ->
		for result in data
			{label: result, value: result}
			
getRequestedItemTemplate = ->
	$.ajax ->
		url: '/Clients/1/RequestedItemTemplate' # The "1" is ignored - hack to keep routes pretty
		cache: false
		dataType: 'html'
		success: (data) ->
			$('#requesteditems').append(data)
			
scheduleClient = (clientId, date) ->
	$.ajax ->
		url: "/Schedule/Create"
		type: "POST"
		data: { id: clientId, scheduledDate: date.toUTCString() }
		dataType: "json"

moveClient = (eventId, dayDelta, minDelta) ->
	$.ajax ->
		url: '/Schedule/Edit',
		cache: false,
		dataType: 'json',
		type: 'POST',
		data: { id: eventId, dayDelta, minDelta }
		
getId = ->
	regex = new RegExp(/\d/)
	id = regex.exec(window.location.pathname)
	id?[0] ? 0

# All the rest of the items run on page load
jQuery ($) ->
	
	# Add nice UI functionality to clear the search box on focus
	addAutoClear $('#search'), DEFAULT_SEARCH_VALUE

	# Pretty up the phone numbers
	$('.phone').mask('(999) 999-9999')
  
	# If there is a Flash Message, fade it in and then out
	$('#flashmessage').fadeIn(1000).delay(4000).fadeOut(1000)
	
	# Add autocomplete functionality to search box
	$('#search').autocomplete ->
		source: (req, add) -> add searchClients(req.term)
		select: (event, ui) -> $('searchClientId').val(ui.item.id)
	
	# Add autocomplete functionality to all Requested Items
	$('td.item_name > input').live 'autocomplete', ->
		source: (req, add) -> add getDistinctItems(req.term)
	
	#Add click functionality to add new Requested Item
	$('#add_item').click(getRequestedItemTemplate)
	
	#Add click functionality to remove an existing Requested Item
	$('a.deleteRow').live 'click', ->
		@parents('tr:first').remove()
	
	# Add functionality to generate the nice jQuery calendar and load the events
	$('#calendar').fullCalendar ->
		eventSources: [
				url: 'http://www.google.com/calendar/feeds/usa__en%40holiday.calendar.google.com/public/basic',
				type: 'GET',
				color: '#F0E68C',
				textColor: '#666'
			,
				url: '/Schedule/CalendarEvents',
				type: 'GET',
				dataType: 'json'
				data: { id : getId() }
		]
		header:
			left: 'today',
			center: 'title',
			right: 'today prev,next'
		drop: (date, allDay) ->
			eventObject = @data('eventObject')
			eventObject.start = date
			eventObject.allDay = false
			eventObject.editable = true
			clientId = @attr('id').substring(5) 			#Grab the ID from the div
			scheduleClient clientId, date
			$('#calendar').fullCalendar('renderEvent', eventObject, true)
			@remove()
		eventDrop: (event, dayDelta, minuteDelta, allDay, revertFunc) ->
			if confirm "Are you sure you want to move this appointment?"
				moveClient event.id, dayDelta, minuteDelta
			else
				revertFunc()
		disableResizing: true
		editable: true
		droppable: true
		defaultEventMinutes: 30
		firstDay: 1
		defaultView: 'agendaWeek'
		allDayDefault: false
		ignoreTimezone: false
		allDayText: 'Holidays' # Figuring all day events are imported from Google only
		minTime: 5
		maxTime: 24
		
		

	
