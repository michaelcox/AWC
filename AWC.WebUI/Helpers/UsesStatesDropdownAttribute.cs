using System.Linq;
using System.Web.Mvc;
using AWC.Domain.Abstract;
using AWC.Domain.Entities;
using Ninject;

namespace AWC.WebUI.Helpers
{
    public class UsesStatesDropdownAttribute : ActionFilterAttribute
    {
        [Inject]
        public IRepository Repository { get; set; }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            ViewResult viewResult = filterContext.Result as ViewResult;
            if (viewResult != null)
            {
                viewResult.ViewData["UsStates"] = Repository.All<UsState>().OrderBy(s => s.StateName).ToList();
            }
        }
    }
}