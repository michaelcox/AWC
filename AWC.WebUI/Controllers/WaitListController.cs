using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AWC.Domain.Abstract;
using AWC.Domain.Entities;
using AWC.WebUI.Infrastructure.Logging;

namespace AWC.WebUI.Controllers
{
    public class WaitListController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;

        public WaitListController(IRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public ActionResult Index()
        {
            // Select all clients with no active appointments in the future
            var clients = from c in _repository.All<Client>()
                          join a in _repository.All<Appointment>() on c.ClientId equals a.ClientId
                          where a.IsActive
                          select c;

            return View();
        }

    }
}
