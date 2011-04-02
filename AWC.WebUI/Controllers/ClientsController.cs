using System.Web.Mvc;
using AWC.Domain.Abstract;
using AWC.Domain.Entities;
using AWC.WebUI.Models;
using Omu.ValueInjecter;

namespace AWC.WebUI.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IRepository _repository;

        public ClientsController(IRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Create()
        {
            ClientEditViewModel clientEditViewModel = new ClientEditViewModel();
            clientEditViewModel.UsStates = new SelectList(_repository.All<UsState>(), "StateCode", "StateName");
            return View(clientEditViewModel);
        } 

        [HttpPost]
        public ActionResult Create(ClientEditViewModel clientEditViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Client client = new Client();
                    client.InjectFrom(clientEditViewModel);
                    _repository.Add(client);
                    _repository.CommitChanges();
                    // TODO: Add a message confirming that the add was made
                    return RedirectToAction("Edit", new {id = client.ClientId});
                }
            }
            catch
            {
                return View(clientEditViewModel);
            }
            return View(clientEditViewModel);
        }
        
        public ActionResult Edit(int id)
        {
            Client client = _repository.Single<Client>(c => c.ClientId == id);
            ClientEditViewModel clientEditViewModel = new ClientEditViewModel
                                                          {
                                                              UsStates = new SelectList(_repository.All<UsState>(), "StateCode","StateName")
                                                          };

            clientEditViewModel.InjectFrom(client);

            return View(clientEditViewModel);
        }

        [HttpPost]
        public ActionResult Edit(ClientEditViewModel clientEditViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Client client = _repository.Single<Client>(c => c.ClientId == clientEditViewModel.ClientId);
                    client.InjectFrom(clientEditViewModel);
                    _repository.CommitChanges();
                    // TODO: Add a message confirming that the update was made
                    return RedirectToAction("Edit", new { id = client.ClientId });
                }
            }
            catch
            {
                return View(clientEditViewModel);
            }
            return View(clientEditViewModel);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                Client client = _repository.Single<Client>(c => c.ClientId == id);
                _repository.Delete(client);
                _repository.CommitChanges();
                return RedirectToAction("Create");
            }
            catch
            {
                // TODO: Add a message explaining we were unable to delete
                return RedirectToAction("Edit", new { id = id});
            }
        }
    }
}
