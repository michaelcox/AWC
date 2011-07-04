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
            var clients = from c in _repository.All<Client>().Include("ClientNotes")
                          join a in _repository.All<Appointment>().Include("RequestedItems") on c equals a.Client
                          where (a.AppointmentStatusId == (byte)Constants.AppointmentStatusId.Scheduled || a.AppointmentStatusId == (byte)Constants.AppointmentStatusId.Rescheduled)
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

            var scheduledViewModel = new ScheduleViewModel {Clients = clients};

            return View(scheduledViewModel);
        }

        public ActionResult WaitList()
        {
            // Load clients/appointments with eager loading
            var clients = from c in _repository.All<Client>().Include("ClientNotes")
                          join a in _repository.All<Appointment>().Include("RequestedItems") on c equals a.Client
                          where a.AppointmentStatusId == (byte)Constants.AppointmentStatusId.NotScheduled
                          orderby  a.CreatedDateTime ascending 
                          select new WaitlistClient
                                     {
                                         ClientId = c.ClientId,
                                         CreatedDateTime = c.CreatedDateTime,
                                         FirstName = c.FirstName,
                                         LastName = c.LastName,
                                         PrimaryPhoneNumber = c.PrimaryPhoneNumber,
                                         PhoneNumberTypeId = c.PrimaryPhoneTypeId,
                                         RequestedItems = a.RequestedItems,
                                         ClientNotes = c.ClientNotes
                                     };

            var waitlistViewModel = new WaitListViewModel {Clients = clients};

            return View(waitlistViewModel);
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

    }
}
