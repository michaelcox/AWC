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
                    TempData["success"] = string.Format("A new client record for {0} {1} has been created successfully.", client.FirstName, client.LastName);
                    return RedirectToAction("Edit", new { id = client.ClientId });
                }
            }
            catch
            {
                TempData["error"] = "There was an error while trying to create the client record.";
                return View(clientEditViewModel);
            }

            TempData["error"] = "There were validation errors while trying to create the client record.";
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

                    TempData["success"] = string.Format("The client record for {0} {1} has been updated successfully.", client.FirstName, client.LastName);
                    return RedirectToAction("Edit", new { id = client.ClientId });
                }
            }
            catch
            {
                TempData["error"] = "There was an error while trying to edit the client record.";
                return View(clientEditViewModel);
            }

            TempData["error"] = "There were validation errors while trying to edit the client record.";
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
                TempData["success"] = string.Format("The client record for {0} {1} has been deleted.", client.FirstName, client.LastName);
                return RedirectToAction("Create");
            }
            catch
            {
                TempData["error"] = "There was an error while trying to delete the client record.";
                return RedirectToAction("Edit", new { id });
            }
        }
    }
}
