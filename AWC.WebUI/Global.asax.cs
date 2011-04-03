using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using AWC.Domain.Abstract;
using AWC.Domain.Concrete;
using AWC.WebUI.Infrastructure.Logging;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Mvc;

namespace AWC.WebUI
{
    public class MvcApplication : NinjectHttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Clients", action = "Create", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            Initialize.Init(ConfigurationManager.ConnectionStrings["AWCDatabase"].ConnectionString);
        }

        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            return kernel;
        }

    }

    public class SiteModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRepository>().To<EFRepository>().InRequestScope();
            Bind<ILogger>().To<NLogLogger>().InRequestScope();
        }
    }

}