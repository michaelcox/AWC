using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AWC.Domain;
using AWC.Domain.Abstract;
using AWC.Domain.Entities;
using AWC.WebUI.Infrastructure.Logging;
using AWC.WebUI.Models;

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

        public ActionResult Confirmed()
        {
            // Load clients/appointments with eager loading
            var clients = GetScheduledClients();

            var scheduledViewModel = new ScheduleViewModel {Clients = clients};

            return View(scheduledViewModel);
        }

        public ActionResult Calendar()
        {
            return View();
        }

        public JsonResult CalendarEvents(int? start, int? end)
        {
            DateTime t = DateTime.Today;

            var startDate = start.HasValue ? ConvertFromUnixTimestamp(start.Value) : DateTime.Today;
            var endDate = end.HasValue
                              ? ConvertFromUnixTimestamp(end.Value)
                              : new DateTime(t.Year, t.Month, 1, 11, 59, 59).AddMonths(3).AddDays(-1);

            var clients = GetScheduledClients();

            // Pull events object specific to jQuery calendar format
            var events = from c in clients.Where(client => client.ScheduledDateTime >= startDate && client.ScheduledDateTime <= endDate).ToList()
                         select new
                                    {
                                        id = c.ClientId,
                                        title = c.FirstName + " " + c.LastName,
                                        start = c.ScheduledDateTime.ToString("s")
                                    };



            return Json(events, JsonRequestBehavior.AllowGet);
            //return string.Format("{0}({1});", callback, "");
        }

        public ActionResult Create(int clientId)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit()
        {
            return View();
        }

        private IEnumerable<ScheduledClient> GetScheduledClients()
        {
            return from c in _repository.All<Client>().Include("ClientNotes")
                   join a in _repository.All<Appointment>().Include("RequestedItems") on c equals a.Client
                   where
                       (a.AppointmentStatusId == (byte) Constants.AppointmentStatusId.Scheduled ||
                        a.AppointmentStatusId == (byte) Constants.AppointmentStatusId.Rescheduled)
                       && a.ScheduledDateTime.HasValue
                   orderby a.ScheduledDateTime descending
                   select new ScheduledClient
                              {
                                  ClientId = c.ClientId,
                                  FirstName = c.FirstName,
                                  LastName = c.LastName,
                                  PrimaryPhoneNumber = c.PrimaryPhoneNumber,
                                  PhoneNumberTypeId = c.PrimaryPhoneTypeId,
                                  ScheduledDateTime = a.ScheduledDateTime.Value,
                                  TwoDayConfirmation = a.TwoDayConfirmation,
                                  TwoWeekConfirmation = a.TwoWeekConfirmation,
                                  SentLetterOrEmail = a.SentLetterOrEmail,
                                  RequestedItems = a.RequestedItems,
                                  ClientNotes = c.ClientNotes
                              };
        }

        private string FormatDateToISO8601(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd") + "T" + dateTime.ToString("hh:mm:ss") + "Z";
        }

        private DateTime ConvertFromUnixTimestamp(int timestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dateTime.AddSeconds(timestamp);
        }
    }
}
