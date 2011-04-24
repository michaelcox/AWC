using System.Linq;
using System.Web.Mvc;
using AWC.Domain.Abstract;
using AWC.Domain.Entities;
using Ninject;

namespace AWC.WebUI.Helpers
{
    public class UsesCountiesDropdownAttribute : ActionFilterAttribute
    {
        [Inject]
        public IRepository Repository { get; set; }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            ViewResult viewResult = filterContext.Result as ViewResult;
            if (viewResult != null)
            {
                viewResult.ViewData["Counties"] = Repository.All<County>().OrderBy(c => c.CountyName).ToList();
            }
        }
    }
}