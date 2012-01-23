using System.Linq;
using System.Web.Mvc;
using AWC.Domain.Abstract;
using AWC.Domain.Entities;
using Ninject;

namespace AWC.WebUI.Helpers
{
    public class UsesPartnerOrgsDropdownAttribute : ActionFilterAttribute
    {
        [Inject]
        public IRepository Repository { get; set; }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            ViewResult viewResult = filterContext.Result as ViewResult;
            if (viewResult != null)
            {
                viewResult.ViewData["PartneringOrgs"] =
                    Repository.All<Client>().Where(c => c.PartneringOrganization != null).Select(c => c.PartneringOrganization).Distinct().Take(10).ToList();
            }
        }
    }
}