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
    public class WaitlistController : Controller
    {

        private readonly IRepository _repository;
        private readonly ILogger _logger;

        public WaitlistController(IRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public ActionResult Index()
        {
            // Load clients/appointments with eager loading
            var clients = (from c in _repository.All<Client>()
                          join a in _repository.All<Appointment>() on c equals a.Client
                          where a.AppointmentStatusId == (byte)Constants.AppointmentStatusId.NotScheduled
                          orderby a.CreatedDateTime ascending
                          select new WaitlistClient
                          {
                              ClientId = c.ClientId,
                              AppointmentId = a.AppointmentId,
                              CreatedDateTime = c.CreatedDateTime,
                              FirstName = c.FirstName,
                              LastName = c.LastName,
                              PrimaryPhoneNumber = c.PrimaryPhoneNumber,
                              PhoneNumberTypeId = c.PrimaryPhoneTypeId
                          }).ToList();

            var requestedItems = _repository.All<RequestedItem>().ToList();

            foreach (var client in clients)
            {
                client.RequestedItems = new List<RequestedItem>();
                client.RequestedItems.AddRange(requestedItems.Where(r => r.AppointmentId == client.AppointmentId));
            }

            var waitlistViewModel = new WaitListViewModel { Clients = clients };

            return View(waitlistViewModel);
        }

    }
}
