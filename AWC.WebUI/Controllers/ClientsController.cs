using System;
using System.Web.Mvc;
using AWC.Domain.Abstract;
using AWC.Domain.Entities;
using AWC.WebUI.Infrastructure.Logging;
using AWC.WebUI.Models;
using Omu.ValueInjecter;

namespace AWC.WebUI.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;

        public ClientsController(IRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public ActionResult Create()
        {
            ClientEditViewModel clientEditViewModel = new ClientEditViewModel
                                                          {
                                                              UsStates = new SelectList(_repository.All<UsState>(), "StateCode","StateName")
                                                          };
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
                    this.FlashSuccess(string.Format("A new client record for {0} {1} has been created successfully.", client.FirstName, client.LastName));
                    return RedirectToAction("Edit", new { id = client.ClientId });
                }
                this.FlashError("There were validation errors while trying to create the client record.");
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
                this.FlashError("There was an error while trying to create the client record.");
                return View(clientEditViewModel);
            }
            
            clientEditViewModel.UsStates = new SelectList(_repository.All<UsState>(), "StateCode", "StateName");
            return View(clientEditViewModel);
        }
        
        public ActionResult Edit(int id)
        {
            try
            {
                Client client = _repository.Single<Client>(c => c.ClientId == id);
                ClientEditViewModel clientEditViewModel = new ClientEditViewModel
                {
                    UsStates = new SelectList(_repository.All<UsState>(), "StateCode", "StateName")
                };
                clientEditViewModel.InjectFrom(client);
                return View(clientEditViewModel);
            }
            catch (Exception ex)
            {
                this.FlashError("Unable to load client record from the database.");
                _logger.Fatal(ex);
                return RedirectToAction("Create");
            }
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
                    this.FlashSuccess(string.Format("The client record for {0} {1} has been updated successfully.", client.FirstName, client.LastName));
                    return RedirectToAction("Edit", new { id = client.ClientId });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                this.FlashError("There was an error while trying to edit the client record.");
            }

            this.FlashError("There were validation errors while trying to edit the client record.");
            clientEditViewModel.UsStates = new SelectList(_repository.All<UsState>(), "StateCode", "StateName");
            return View(clientEditViewModel);

        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                Client client = _repository.Single<Client>(c => c.ClientId == id);
                string firstName = client.FirstName;
                string lastName = client.LastName;
                _repository.Delete(client);
                _repository.CommitChanges();
                this.FlashSuccess(string.Format("The client record for {0} {1} has been deleted.", firstName, lastName));
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                this.FlashError("There was an error while trying to delete the client record.");
                return RedirectToAction("Edit", new { id });
            }
        }
    }
}
