using System;
using System.Linq;
using System.Web.Mvc;
using AWC.Domain;
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
            client.IsReplacingFurniture = false; // To be set on Partner Info page
            client.CreatedDateTime = DateTime.UtcNow;
            client.LastUpdatedDateTime = DateTime.UtcNow;
            try
            {
                if (ModelState.IsValid)
                {
                    _repository.Add(client);

                    // On first create, add blank appointment
                    var appt = new Appointment
                                   {
                                       ClientId = client.ClientId,
                                       AppointmentStatusId = (byte) Constants.AppointmentStatusId.NotScheduled,
                                       SentLetterOrEmail = false
                                   };

                    _repository.Add(appt);
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

            var clientEditViewModel = new ClientEditViewModel();
            clientEditViewModel.InjectFrom(client);
            return View(clientEditViewModel);
        }

        [UsesCountiesDropdown]
        [UsesStatesDropdown]
        public ActionResult BasicInfo(int id)
        {
            try
            {
                var client = _repository.Single<Client>(c => c.ClientId == id);

                // Load the current appointment
                var appt = _repository.Single<Appointment>(a => a.ClientId == id && a.AppointmentId != (byte)Constants.AppointmentStatusId.Closed);

                var clientEditViewModel = new ClientEditViewModel();

                clientEditViewModel.InjectFrom(client);
                clientEditViewModel.InjectFrom(appt);
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
                var existingClient = _repository.Single<Client>(c => c.ClientId == client.ClientId);
                if (ModelState.IsValid)
                {
                    client.CreatedDateTime = existingClient.CreatedDateTime; // Don't want to overwrite this with injection
                    existingClient.InjectFrom(client);
                    existingClient.LastUpdatedDateTime = DateTime.UtcNow;
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

            var clientEditViewModel = new ClientEditViewModel();
            clientEditViewModel.InjectFrom(client);
            return View(clientEditViewModel);
        }

        [UsesPartnerOrgsDropdown]
        public ActionResult PartnerInfo(int id)
        {
            try
            {
                Client client = _repository.Single<Client>(c => c.ClientId == id);

                var partnerInfoViewModel = new PartnerInfoViewModel
                {
                    IsReplacingFurniture = client.IsReplacingFurniture,
                    ClientId = client.ClientId,
                    ClientFirstName = client.FirstName,
                    ClientLastName = client.LastName
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
                    Caseworker caseworker;
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

                    var client = _repository.Single<Client>(c => c.ClientId == partnerInfoViewModel.ClientId);
                    client.IsReplacingFurniture = partnerInfoViewModel.IsReplacingFurniture;
                    client.CaseworkerId = caseworker.CaseworkerId;
                    client.LastUpdatedDateTime = DateTime.UtcNow;

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
    
        [ChildActionOnly]
        public ActionResult ClientNotes(int clientId, string refAction)
        {
            ClientNotesViewModel clientNotesViewModel = new ClientNotesViewModel
            {
                ClientId = clientId,
                ClientNotes = _repository.All<ClientNote>().Where(c => c.ClientId == clientId).ToList(),
                RefAction = refAction
            };

            return View(clientNotesViewModel);
        }

        [HttpPost]
        public ActionResult AddNote(ClientNote note, string refAction)
        {
            note.ClientNoteId = 0;
            note.PostedDateTime = DateTime.UtcNow;
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

            // Adding in some assurances they can't force a bad redirect
            refAction = (refAction == "PartnerInfo") ? "PartnerInfo" : "BasicInfo";
            return RedirectToAction(refAction, new { id = note.ClientId });
        }
    
        [HttpPost]
        public ActionResult AddNewItem()
        {
            return PartialView("EditRequestedItem", new RequestedItem());
        }
    
    }
}
