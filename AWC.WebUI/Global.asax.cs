using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using AWC.Domain.Abstract;
using AWC.Domain.Concrete;
using AWC.WebUI.Infrastructure.Logging;
using AWC.WebUI.Utils;
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
                "Waitlist", // Route name
                "Waitlist", // URL with parameters
                new { controller = "Waitlist", action = "Index" }
            );

            routes.MapRoute(
                "Schedule", // Route name
                "Schedule/{action}/{id}", // URL with parameters
                new { controller = "Schedule", action = "Confirmed", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "CreateClient", // Route name
                "Clients/Create", // URL with parameters
                new { controller = "Clients", action = "Create" }
            );

            routes.MapRoute(
                "ClientInfo", // Route name
                "Clients/{id}", // URL with parameters
                new { controller = "Clients", action = "BasicInfo", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ClientDetail", // Route name
                "Clients/{id}/{action}", // URL with parameters
                new { controller = "Clients"}
            );

            routes.MapRoute(
                "Search", // Route name
                "Search/{action}", // URL with parameters
                new { controller = "Search", action = "Index" }
            );

            routes.MapRoute(
                "Homepage", // Route name
                "", // URL with parameters
                new { controller = "Dashboard", action = "Index" }
            );

        }

        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ModelMetadataProviders.Current = new CustomMetadataProvider();

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