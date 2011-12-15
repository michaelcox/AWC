using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AWC.Domain.Abstract;
using AWC.Domain.Entities;
using AWC.WebUI.Infrastructure.Logging;
using AWC.WebUI.Models;
using Newtonsoft.Json;
using Omu.ValueInjecter;

namespace AWC.WebUI.Controllers
{
    public class SearchController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;

        public SearchController(IRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public JsonResult Index(string q)
        {
            var clientSearchResults = SearchClients(q);
            return Json(clientSearchResults, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public string DistinctItems(string q)
        {
            var items = (from i in _repository.All<RequestedItem>()
                         orderby i.ItemName ascending
                         select i.ItemName);

            if (!string.IsNullOrEmpty(q))
            {
                items = items.Where(i => i == q || i.Contains(q));
            }

            items = items.Distinct();

            return JsonConvert.SerializeObject(items);
        }

        [HttpPost]
        public RedirectToRouteResult Index(int clientId)
        {
            var client = _repository.Single<Client>(c => c.ClientId == clientId);

            return RedirectToAction("BasicInfo", "Clients", new { id = client.ClientId });
        }

        private List<SearchResultViewModel> SearchClients(string searchTerm)
        {
            var clients = (from r in _repository.All<Client>()
                          where (r.EmailAddress.StartsWith(searchTerm)
                                 || r.PrimaryPhoneNumber.StartsWith(searchTerm)
                                 || r.SecondaryPhoneNumber.StartsWith(searchTerm)
                                 || r.FirstName.StartsWith(searchTerm)
                                 || r.LastName.StartsWith(searchTerm))
                          select r).Take(10);

            var clientSearchResults = new List<SearchResultViewModel>();
            foreach (var client in clients)
            {
                var result = new SearchResultViewModel();
                result.InjectFrom(client);
                clientSearchResults.Add(result);
            }

            return clientSearchResults;
        }

    }
}
