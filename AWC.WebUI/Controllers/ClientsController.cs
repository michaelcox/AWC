using System;
using System.Linq;
using System.Web.Mvc;
using AWC.Domain.Abstract;
using AWC.Domain.Entities;
using AWC.WebUI.Helpers;
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

        [UsesCountiesDropdown]
        [UsesStatesDropdown]
        public ActionResult Create()
        {
            return View(new ClientEditViewModel());
        } 

        [HttpPost]
        [UsesCountiesDropdown]
        [UsesStatesDropdown]
        public ActionResult Create(Client client)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    client.IsReplacingFurniture = false; // To be set on Partner Info page
                    _repository.Add(client);
                    _repository.CommitChanges();
                    this.FlashSuccess(string.Format("A new client record for {0} {1} has been created successfully.", client.FirstName, client.LastName));
                    return RedirectToAction("BasicInfo", new { id = client.ClientId });
                }
                this.FlashError("There were validation errors while trying to create the client record.");
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
                this.FlashError("There was an error while trying to create the client record.");
            }

            return View();
        }

        [UsesCountiesDropdown]
        [UsesStatesDropdown]
        public ActionResult BasicInfo(int id)
        {
            try
            {
                Client client = _repository.Single<Client>(c => c.ClientId == id);
                ClientNotesViewModel clientNotesViewModel = new ClientNotesViewModel
                                                                {
                                                                    ClientNotes =
                                                                        _repository.All<ClientNote>().Where(c => c.ClientId == client.ClientId).ToList(),
                                                                        ClientId = client.ClientId
                                                                };
                ClientEditViewModel clientEditViewModel = new ClientEditViewModel
                                                              {
                                                                  ClientNotesViewModel = clientNotesViewModel
                                                              };

                clientEditViewModel.InjectFrom(client);
                clientEditViewModel.RequestedItemsViewModel = new RequestedItemsViewModel();
                return View(clientEditViewModel);
            }
            catch (Exception ex)
            {
                this.FlashError("Unable to load client record from the database.");
                _logger.Error(ex);
                return RedirectToAction("Create");
            }
        }

        [HttpPost]
        [UsesCountiesDropdown]
        [UsesStatesDropdown]
        public ActionResult BasicInfo(Client client)
        {
            try
            {
                Client existingClient = _repository.Single<Client>(c => c.ClientId == client.ClientId);
                if (ModelState.IsValid)
                {
                    existingClient.InjectFrom(client);
                    _repository.CommitChanges();
                    this.FlashSuccess(string.Format("The client record for {0} {1} has been updated successfully.", client.FirstName, client.LastName));
                    return RedirectToAction("BasicInfo", new { id = client.ClientId });
                }

                this.FlashError("There were validation errors while trying to edit the client record.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                this.FlashError("There was an error while trying to edit the client record.");
            }


            ClientNotesViewModel clientNotesViewModel = new ClientNotesViewModel
            {
                ClientNotes = _repository.All<ClientNote>().Where(c => c.ClientId == client.ClientId).ToList(),
                ClientId = client.ClientId
            };
            ClientEditViewModel clientEditViewModel = new ClientEditViewModel
            {
                ClientNotesViewModel = clientNotesViewModel
            };

            clientEditViewModel.InjectFrom(client);
            return View(clientEditViewModel);
        }

        [UsesPartnerOrgsDropdown]
        public ActionResult PartnerInfo(int id)
        {
            try
            {
                Client client = _repository.Single<Client>(c => c.ClientId == id);
                ClientNotesViewModel clientNotesViewModel = new ClientNotesViewModel
                {
                    ClientNotes =
                        _repository.All<ClientNote>().Where(c => c.ClientId == client.ClientId).ToList(),
                    ClientId = client.ClientId
                };

                var partnerInfoViewModel = new PartnerInfoViewModel
                {
                    IsReplacingFurniture = client.IsReplacingFurniture,
                    ClientId = client.ClientId,
                    ClientFirstName = client.FirstName,
                    ClientLastName = client.LastName,
                    ClientNotesViewModel = clientNotesViewModel
                };

                // Client may not have a caseworker assigned if they were only just created
                Caseworker caseworker = client.Caseworker;
                if (caseworker != null)
                {
                    partnerInfoViewModel.InjectFrom(caseworker);
                }

                return View(partnerInfoViewModel);
            }
            catch (Exception ex)
            {
                this.FlashError("Unable to load client record from the database.");
                _logger.Error(ex);
                return RedirectToAction("Create");
            }

        }

        [HttpPost]
        [UsesPartnerOrgsDropdown]
        public ActionResult PartnerInfo(PartnerInfoViewModel partnerInfoViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Caseworker caseworker = null;
                    if (partnerInfoViewModel.CaseworkerId > 0)
                    {
                        caseworker = _repository.Single<Caseworker>(c => c.CaseworkerId == partnerInfoViewModel.CaseworkerId);
                    }
                    else
                    {
                        caseworker = new Caseworker();
                        _repository.Add(caseworker);
                    }

                    caseworker.InjectFrom(partnerInfoViewModel);
                    _repository.CommitChanges();

                    Client client = _repository.Single<Client>(c => c.ClientId == partnerInfoViewModel.ClientId);
                    client.IsReplacingFurniture = partnerInfoViewModel.IsReplacingFurniture;
                    client.CaseworkerId = caseworker.CaseworkerId;

                    _repository.CommitChanges();

                    this.FlashSuccess(string.Format("The client record for {0} {1} has been updated successfully.", client.FirstName, client.LastName));
                    return RedirectToAction("PartnerInfo", new { id = client.ClientId });
                }
            }
            catch(Exception ex)
            {
                this.FlashError("Unable to save changes to client record.");
                _logger.Error(ex);
            }

            ClientNotesViewModel clientNotesViewModel = new ClientNotesViewModel
            {
                ClientNotes =
                    _repository.All<ClientNote>().Where(c => c.ClientId == partnerInfoViewModel.ClientId).ToList(),
                ClientId = partnerInfoViewModel.ClientId
            };
            partnerInfoViewModel.ClientNotesViewModel = clientNotesViewModel;
            return View(partnerInfoViewModel);
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
                return RedirectToAction("BasicInfo", new { id });
            }
        }
    
        [HttpPost]
        public ActionResult AddNote(ClientNote note)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repository.Add(note);
                    _repository.CommitChanges();
                    this.FlashSuccess("Note added successfully.");
                }
                else
                {
                    this.FlashError("There were validation errors while trying to add the note.");
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
                this.FlashError("There was an error while trying add the note.");
            }

            return RedirectToAction("BasicInfo", new { id = note.ClientId });
        }
    
        [HttpPost]
        public ActionResult AddNewItem()
        {
            return PartialView("EditRequestedItem", new RequestedItem());
        }
    
    }
}
