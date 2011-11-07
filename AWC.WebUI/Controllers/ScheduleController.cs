﻿using System;
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

        public JsonResult CalendarEvents(int? start, int? end, int? id)
        {
            DateTime t = DateTime.Today;

            var startDate = start.HasValue ? ConvertFromUnixTimestamp(start.Value).ToUniversalTime() : DateTime.Today.ToUniversalTime();
            var endDate = end.HasValue
                              ? ConvertFromUnixTimestamp(end.Value).ToUniversalTime()
                              : new DateTime(t.Year, t.Month, 1, 11, 59, 59).AddMonths(3).AddDays(-1).ToUniversalTime();

            var clients = GetScheduledClients();

            // Pull events object specific to jQuery calendar format

            var events = from c in clients.Where(client => client.ScheduledDateTime >= startDate && client.ScheduledDateTime <= endDate).ToList()
                             select new
                                        {
                                            id = c.AppointmentId,
                                            title = c.FirstName + " " + c.LastName,
                                            start = TimeHelper.ConvertToLocal(c.ScheduledDateTime).ToString("s"),
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
        public JsonResult Edit(int id, int dayDelta, int minDelta)
        {
            var appt = _repository.Single<Appointment>(a => a.AppointmentId == id);
            if (appt != null && appt.ScheduledDateTime.HasValue)
            {
                var newScheduledTime = appt.ScheduledDateTime.Value.AddDays(dayDelta).AddMinutes(minDelta);
                appt.ScheduledDateTime = newScheduledTime;
                appt.AppointmentStatusId = (byte) Constants.AppointmentStatusId.Rescheduled;
                appt.TwoDayConfirmation = null;
                appt.TwoWeekConfirmation = null;
                _repository.CommitChanges();

                return Json(new {success = true});
            }
            return Json(new { success = false });
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
                                  AppointmentId = a.AppointmentId,
                                  AppointmentStatusId = a.AppointmentStatusId,
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

        private DateTime ConvertFromUnixTimestamp(int timestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dateTime.AddSeconds(timestamp);
        }
    }
}
