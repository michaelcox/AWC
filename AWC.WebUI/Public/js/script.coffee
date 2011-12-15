DEFAULT_SEARCH_VALUE = 'Client Search'

addAutoClear = (inputBox, defaultValue) ->
	inputBox.val(defaultValue)
	
	inputBox.focus ->
		inputBox.val('') if (inputBox.val() is defaultValue)

	inputBox.blur ->
		inputBox.val(defaultValue) if (inputBox.val() is '')
		
searchClients = (req, add) ->
	searchTerm = req.term
	$.getJSON "/search?q=" + searchTerm, (data) ->
		r = for result in data
			name = result.FirstName + " " + result.LastName
			phoneNumber = "(" + result.PrimaryPhoneNumber.substring(0, 3) + ")";
			phoneNumber += " " + result.PrimaryPhoneNumber.substring(3, 6) + "-";
			phoneNumber += result.PrimaryPhoneNumber.substring(6);
			label = name + " - " + phoneNumber
			id = result.ClientId
			{label, value: label, id}
		add(r)
			
getRequestedItemTemplate = ->
	$.ajax
		url: '/Clients/1/RequestedItemTemplate'  # The "1" is ignored - hack to keep routes pretty
		cache: false
		dataType: 'html'
		success: (data) ->
			$('#requesteditems').append(data)
			
scheduleClient = (clientId, date) ->
	$.ajax
		url: "/Schedule/Create"
		type: "POST"
		data: { id: clientId, scheduledDate: date.toUTCString() }
		dataType: "json"

moveClient = (eventId, dayDelta, minDelta) ->
	$.ajax
		url: '/Schedule/Move',
		cache: false,
		dataType: 'json',
		type: 'POST',
		data: { id: eventId, dayDelta, minDelta }
		
getId = ->
	regex = new RegExp(/\d/)
	id = regex.exec(window.location.pathname)
	id?[0] ? 0

updateScheduledDate = ->
	day = $('#scheduledDate').datepicker('getDate')?.getDate()
	month = $('#scheduledDate').datepicker('getDate')?.getMonth() + 1
	year = $('#scheduledDate').datepicker('getDate')?.getFullYear()
	hour = $('#scheduledHour').val()
	min = $('#scheduledMinute').val()
	ampm = $('#scheduledAmPm').val()
	$('#ScheduledDateTime').val(month + '/' + day + '/' + year + ' ' + hour + ':' + min + ' ' + ampm)
	return

editScheduledDateTime = ->
	clientId = $('#ClientId').val()
	newDateTime = new Date($('#ScheduledDateTime').val())
	$.ajax
		url: "/Schedule/Edit"
		type: "POST"
		data: { id: clientId, localDateTime: newDateTime.toUTCString() }
		dataType: "json"
		success: ->
			$('#scheduledDateFlash').html('Appointment date updated!')
			$('#scheduledDateFlash').fadeIn(1000).delay(4000).fadeOut(1000)

jQuery ($) ->
	
	# Add nice UI functionality to clear the search box on focus
	addAutoClear $('#search'), DEFAULT_SEARCH_VALUE

	# Pretty up the phone numbers
	$('.phone').mask('(999) 999-9999')
  
	# If there is a Flash Message, fade it in and then out
	$('#flashmessage').fadeIn(1000).delay(4000).fadeOut(1000)
	
	# Add autocomplete functionality to search box
	$('#search').autocomplete
		source: searchClients
		select: (event, ui) -> $('#searchClientId').val(ui.item.id)
		autoFocus: true
	
	# Add autocomplete functionality to all Requested Items
	$('td.item_name > input').live 'keyup.autocomplete', ->
		$(this).autocomplete
			source: window.distinctItems
			delay: 0
			minLength: 1
			autoFocus: true
	
	#Add click functionality to add new Requested Item
	$('#add_item').click (e) ->
		e.preventDefault()
		getRequestedItemTemplate()
	
	#Add click functionality to remove an existing Requested Item
	$('a.deleteRow').live 'click', (e) ->
		e.preventDefault()
		$(this).parents('tr:first').remove()
	
	# Add functionality to generate the nice jQuery calendar and load the events
	$('#calendar').fullCalendar
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
			eventObject = $(this).data('eventObject')
			eventObject.start = date
			eventObject.allDay = false
			eventObject.editable = true
			clientId = $(this).attr('id').substring(5) 			#Grab the ID from the div
			scheduleClient clientId, date
			$('#calendar').fullCalendar('renderEvent', eventObject, true)
			$(this).remove()
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
		
	# Add ability to drag waitlist-clients
	$('#waitlist-clients .waitlist-client').each ->
		# Use the element's text as the event title
		eventObject = { title: $.trim $(this).text() }

		# Store the Event Object in the DOM element so we can get to it later
		$(this).data 'eventObject', eventObject

		# Make the event draggable using jQuery UI
		$(this).draggable
			zIndex: 999
			revert: true		# Will cause the even to go back to its
			revertDuration: 0	# original position after the drag

	# Add warning message if someone uses the search form incorrectly
	$('#searchForm').submit ->
		if $('#searchClientId').val() == ""
			alert('You must select a client from the dropdown options.')
			return false

	# Use the jQuery UI date picker for Appointment Quick View
	$('#scheduledDate').datepicker
		defaultDate: $('#ScheduledDateTime').val()
		onSelect: updateScheduledDate
		
	# Populate the Scheduled Date dropdown lists with times
	for hour in [1..12]
		$('#scheduledHour').append $("<option></option>").attr('value', hour).text(hour)

	for min in ['00', '30']
		$('#scheduledMinute').append $("<option></option>").attr('value', min).text(min)

	# Set the initial value of the dropdown boxes for hour/min/ampm
	initialDate = new Date($('#ScheduledDateTime').val())
	$('#scheduledMinute').val(initialDate.getMinutes())
	if (initialDate.getHours() <= 12)
		$('#scheduledHour').val(initialDate.getHours())
		$('#scheduledAmPm').val('AM')
	else
		$('#scheduledHour').val(initialDate.getHours() - 12)
		$('#scheduledAmPm').val('PM')
	
	# Set the onChange event for the time dropdowns to set the hidden field
	$('#scheduledHour').change updateScheduledDate
	$('#scheduledMinute').change updateScheduledDate
	$('#scheduledAmPm').change updateScheduledDate

	# Add binding when you update the date
	$('#updateScheduledDate').click editScheduledDateTime 

	return # Not necessary, but I think it's cleaner