using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AWC.Domain;
using AWC.Domain.Abstract;
using AWC.Domain.Entities;
using AWC.WebUI.Helpers;
using AWC.WebUI.Infrastructure.Logging;
using AWC.WebUI.Models;
using Omu.ValueInjecter;

namespace AWC.WebUI.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;

        public ScheduleController(IRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public ActionResult Index()
        {
            return RedirectToActionPermanent("ThisWeek");
        }

        public ActionResult ThisWeek()
        {
            DateTime endDate;

            // "This Week" means Sunday, unless today is Sunday
            if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
            {
                endDate = DateTime.Today.AddDays(7);
            }
            else
            {
                DateTime tmpDate = DateTime.Today;
                while (tmpDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    tmpDate = tmpDate.AddDays(1);
                }
                endDate = tmpDate;
            }
            return View("Confirmed", GetScheduleForDates(DateTime.Today, endDate));
        }

        public ActionResult Today()
        {
            return View("Confirmed", GetScheduleForDates(DateTime.Today, DateTime.Today));
        }

        public ActionResult ThisMonth()
        {
            DateTime startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
            DateTime endDate =  new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1, 0, 0, 0).AddMonths(1).AddDays(-1);

            return View("Confirmed", GetScheduleForDates(startDate, endDate));
        }

        public ActionResult ThisYear()
        {
            DateTime startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
            DateTime endDate = new DateTime(DateTime.Today.Year, 12, 31, 23, 59, 59);

            return View("Confirmed", GetScheduleForDates(startDate, endDate));
        }

        public ActionResult Calendar(int? id)
        {
            DateTime? focusDateTime = null;
            if (id.HasValue)
            {
                var clients = GetScheduledClients(DateTime.Today, DateTime.Today.AddYears(1));
                focusDateTime = clients.Where(c => c.ClientId == id).First().ScheduledDateTime;
            }
            return View(focusDateTime);
        }

        public JsonResult CalendarEvents(int? start, int? end, int? id)
        {
            DateTime t = DateTime.Today;

            var startDate = start.HasValue ? ConvertFromUnixTimestamp(start.Value).ToUniversalTime() : DateTime.Today.ToUniversalTime();
            var endDate = end.HasValue
                              ? ConvertFromUnixTimestamp(end.Value).ToUniversalTime()
                              : new DateTime(t.Year, t.Month, 1, 11, 59, 59).AddMonths(3).AddDays(-1).ToUniversalTime();

            var clients = GetScheduledClients(DateTime.Today, DateTime.Today.AddYears(1));

            // Pull events object specific to jQuery calendar format

            var events = from c in clients.Where(client => client.ScheduledDateTime >= startDate && client.ScheduledDateTime <= endDate).ToList()
                             select new
                                        {
                                            id = c.AppointmentId,
                                            title = c.FirstName + " " + c.LastName,
                                            allDay = false,
                                            start = c.ScheduledDateTime.ConvertToLocal().ToString("s"),
                                            editable = true,
                                            url = Url.Action("BasicInfo", "Clients", new { id = c.ClientId }),
                                            color = (id.HasValue && id.Value == c.ClientId) ? "#F16BB4" : "#14AFDB"
                                        };

            return Json(events, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(int id)
        {
            var client = _repository.Single<Client>(c => c.ClientId == id);

            var clientSearchResult = new SearchResultViewModel();
            clientSearchResult.InjectFrom(client);

            return View(clientSearchResult);
        }

        [HttpPost]
        public JsonResult Create(int id, DateTime scheduledDate)
        {
            try
            {
                var appt = _repository.Single<Appointment>(a => a.ClientId == id && a.AppointmentStatusId == (byte)Constants.AppointmentStatusId.NotScheduled);

                if (appt == null) return Json(new { success = false });

                appt.AppointmentStatusId = (byte) Constants.AppointmentStatusId.Scheduled;
                appt.ScheduledDateTime = scheduledDate.ToUniversalTime();
                _repository.CommitChanges();

                return Json(new {success = true});
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult Move(int id, int dayDelta, int minDelta)
        {
            var appt = _repository.Single<Appointment>(a => a.AppointmentId == id);
            if (appt != null && appt.ScheduledDateTime.HasValue)
            {
                var newScheduledTime = appt.ScheduledDateTime.Value.AddDays(dayDelta).AddMinutes(minDelta);

                // If not a change within the same day, create a copy with a status of Rescheduled
                if (newScheduledTime.Date != appt.ScheduledDateTime.Value.Date)
                {
                    var rescheduledAppointment = new Appointment();
                    rescheduledAppointment.InjectFrom(appt);
                    rescheduledAppointment.AppointmentId = 0;
                    rescheduledAppointment.AppointmentStatusId = (byte)Constants.AppointmentStatusId.Rescheduled;
                    _repository.Add(rescheduledAppointment);
                }

                appt.ScheduledDateTime = newScheduledTime;
                appt.AppointmentStatusId = (byte) Constants.AppointmentStatusId.Scheduled;
                appt.TwoDayConfirmation = null;
                appt.TwoWeekConfirmation = null;
                _repository.CommitChanges();

                return Json(new {success = true});
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult Edit(int id, string dateString)
        {
            var utcDateTime = DateTime.Parse(dateString).ConvertToUtc();
            var appt = _repository.Single<Appointment>(a => a.AppointmentId == id);

            // If not a change within the same day, create a copy with a status of Rescheduled
            if (utcDateTime.Date != appt.ScheduledDateTime.Value.Date)
            {
                var rescheduledAppointment = new Appointment();
                rescheduledAppointment.InjectFrom(appt);
                rescheduledAppointment.AppointmentId = 0;
                rescheduledAppointment.AppointmentStatus = null;
                rescheduledAppointment.AppointmentStatusId = (byte) Constants.AppointmentStatusId.Rescheduled;
                _repository.Add(rescheduledAppointment);
            }

            // Update existing appointment
            appt.ScheduledDateTime = utcDateTime;
            appt.AppointmentStatusId = (byte)Constants.AppointmentStatusId.Scheduled;
            appt.TwoDayConfirmation = null;
            appt.TwoWeekConfirmation = null;
            appt.CreatedDateTime = DateTime.UtcNow;

            _repository.CommitChanges();

            return Json(new {success = true});
        }

        private ScheduleViewModel GetScheduleForDates(DateTime startDateTime, DateTime endDateTime)
        {
            // Put Start at the beginning of the day, End at the end
            startDateTime = new DateTime(startDateTime.Year, startDateTime.Month, startDateTime.Day, 0, 0, 0);
            endDateTime = new DateTime(endDateTime.Year, endDateTime.Month, endDateTime.Day, 23, 59, 59);

            // Load clients/appointments with eager loading
            List<ScheduledClient> clients = GetScheduledClients(startDateTime, endDateTime);

            var requestedItems = _repository.All<RequestedItem>().ToList();

            foreach (var client in clients)
            {
                client.RequestedItems = new List<RequestedItem>();
                client.RequestedItems.AddRange(requestedItems.Where(r => r.AppointmentId == client.AppointmentId));
            }

            var scheduledViewModel = new ScheduleViewModel { Clients = clients };

            return scheduledViewModel;
        }

        private List<ScheduledClient> GetScheduledClients(DateTime startDateTime, DateTime endDateTime)
        {
            var s = from c in _repository.All<Client>()
                    join a in _repository.All<Appointment>() on c equals a.Client
                    where
                        a.AppointmentStatusId == (byte) Constants.AppointmentStatusId.Scheduled
                        && a.ScheduledDateTime.HasValue
                        && a.ScheduledDateTime >= startDateTime && a.ScheduledDateTime <= endDateTime
                    orderby a.ScheduledDateTime ascending 
                    select new ScheduledClient
                               {
                                   ClientId = c.ClientId,
                                   AppointmentId = a.AppointmentId,
                                   AppointmentStatusId = a.AppointmentStatusId,
                                   FirstName = c.FirstName,
                                   LastName = c.LastName,
                                   PrimaryPhoneNumber = c.PrimaryPhoneNumber,
                                   PhoneNumberTypeId = c.PrimaryPhoneTypeId,
                                   ScheduledDateTime = a.ScheduledDateTime.Value,
                                   TwoDayConfirmation = a.TwoDayConfirmation,
                                   TwoWeekConfirmation = a.TwoWeekConfirmation,
                                   SentLetterOrEmail = a.SentLetterOrEmail
                               };

            return s.ToList();
        }

        private DateTime ConvertFromUnixTimestamp(int timestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dateTime.AddSeconds(timestamp);
        }
    }
}
