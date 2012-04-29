set PATH=%~dp0;"C:\Program Files\Microsoft\Microsoft Ajax Minifier"

ajaxmin -clobber AWC.WebUI\Public\js\script.js -o AWC.WebUI\Public\js\script.min.js

ajaxmin -clobber AWC.WebUI\Public\js\libs\fullcalendar.js AWC.WebUI\Public\js\libs\gcal.js AWC.WebUI\Public\js\libs\jquery.maskedinput-1.3.js AWC.WebUI\Public\js\libs\jquery.validate.js AWC.WebUI\Public\js\libs\jquery.validate.unobtrusive.js -o AWC.WebUI\Public\js\plugins.min.js

ajaxmin -clobber AWC.WebUI\Public\css\fullcalendar.css AWC.WebUI\Public\css\global.css -o AWC.WebUI\Public\css\all.min.css

pause