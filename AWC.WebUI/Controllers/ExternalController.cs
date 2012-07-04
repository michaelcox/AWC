using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AWC.WebUI.Models;

namespace AWC.WebUI.Controllers
{
    public class ExternalController : Controller
    {

        public ActionResult PartnerForm()
        {
            return View(new PartnerFormViewModel());
        }


    }
}
