using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AWC.WebUI.Controllers
{
    public class ScheduleController : Controller
    {
        //
        // GET: /Schedule/

        public ActionResult Index()
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
