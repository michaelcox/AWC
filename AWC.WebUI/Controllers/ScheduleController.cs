using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WaitList()
        {
            var clientAppointments = from c in _repository.All<Client>()
                                     join a in _repository.All<Appointment>() on c.ClientId equals a.ClientId
                                     select new WaitListViewModel
                                                {
                                                    ClientId = c.ClientId,
                                                    FirstName = c.FirstName,
                                                    LastName = c.LastName,
                                                    ClientNotes = c.ClientNotes.ToList(),
                                                    RequestedItems = a.RequestedItems.ToList()
                                                };

            return View(clientAppointments);
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
