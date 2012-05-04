using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text.RegularExpressions;
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
        [UsesPhoneTypeDropdown]
        public ActionResult Create()
        {
            return View(new ClientEditViewModel());
        }

        [HttpPost]
        [UsesCountiesDropdown]
        [UsesStatesDropdown]
        [UsesPhoneTypeDropdown]
        public ActionResult Create(ClientEditViewModel client)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newClient = new Client();
                    newClient.InjectFrom(client);
                    newClient.CreatedDateTime = DateTime.UtcNow;
                    newClient.LastUpdatedDateTime = DateTime.UtcNow;

                    // Strip out extra characters from phone numbers
                    newClient.PrimaryPhoneNumber = (!string.IsNullOrEmpty(newClient.PrimaryPhoneNumber))
                                                       ? Regex.Replace(newClient.PrimaryPhoneNumber, @"\D", string.Empty)
                                                       : null;
                    newClient.SecondaryPhoneNumber = (!string.IsNullOrEmpty(newClient.SecondaryPhoneNumber))
                                                         ? Regex.Replace(newClient.SecondaryPhoneNumber, @"\D",
                                                                         string.Empty)
                                                         : null;

                    _repository.Add(newClient);

                    // On first create, add blank appointment
                    var appt = new Appointment
                                   {
                                       ClientId = newClient.ClientId,
                                       AppointmentStatusId = (byte) Constants.AppointmentStatusId.NotScheduled,
                                       CreatedDateTime = DateTime.UtcNow,
                                       SentLetterOrEmail = false
                                   };

                    _repository.Add(appt);
                    _repository.CommitChanges();
                    this.FlashSuccess(string.Format("A new client record for {0} {1} has been created successfully.",
                                                    newClient.FirstName, newClient.LastName));
                    return RedirectToAction("BasicInfo", new {id = newClient.ClientId});
                }
                this.FlashError("There were validation errors while trying to create the client record.");
                _logger.Error(ModelState);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                this.FlashError("There was an error while trying to create the client record.");
            }

            return View(client);
        }

        [UsesCountiesDropdown]
        [UsesStatesDropdown]
        [UsesPhoneTypeDropdown]
        public ActionResult BasicInfo(int id)
        {
            try
            {
                var client = _repository.Single<Client>(c => c.ClientId == id);

                // Load the current appointment
                var appt =
                    _repository.All<Appointment>().Where(a => a.ClientId == id).OrderByDescending(a => a.CreatedDateTime).FirstOrDefault();

                var clientEditViewModel = new ClientEditViewModel();

                clientEditViewModel.InjectFrom(client);
                if (appt != null)
                {
                    clientEditViewModel.InjectFrom(appt);
                }
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
        [UsesPhoneTypeDropdown]
        public ActionResult BasicInfo(ClientEditViewModel client)
        {
            try
            {
                // Strip out extra characters from phone numbers
                client.PrimaryPhoneNumber = (!string.IsNullOrEmpty(client.PrimaryPhoneNumber))
                                                ? Regex.Replace(client.PrimaryPhoneNumber, @"\D", string.Empty)
                                                : null;
                client.SecondaryPhoneNumber = (!string.IsNullOrEmpty(client.SecondaryPhoneNumber))
                                                  ? Regex.Replace(client.SecondaryPhoneNumber, @"\D", string.Empty)
                                                  : null;

                var existingClient = _repository.Single<Client>(c => c.ClientId == client.ClientId);
                if (ModelState.IsValid)
                {
                    existingClient.InjectFrom(client);
                    existingClient.LastUpdatedDateTime = DateTime.UtcNow;
                    _repository.CommitChanges();
                    this.FlashSuccess(string.Format("The client record for {0} {1} has been updated successfully.",
                                                    client.FirstName, client.LastName));
                    return RedirectToAction("BasicInfo", new {id = client.ClientId});
                }

                this.FlashError("There were validation errors while trying to edit the client record.");
                _logger.Error(ModelState);
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _logger.Error(string.Format("Property: {0} Error: {1}", validationError.PropertyName,
                                                    validationError.ErrorMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                this.FlashError("There was an error while trying to edit the client record.");
            }

            return View(client);
        }

        [UsesPartnerOrgsDropdown]
        [UsesEthnicities]
        [UsesOtherReferrals]
        public ActionResult DemographicInfo(int id)
        {
            try
            {
                var client = _repository.Single<Client>(c => c.ClientId == id);

                var incomes = _repository.All<ResidentIncome>().Where(i => i.ClientId == client.ClientId).ToList();

                var ethnicities =
                    _repository.All<Ethnicity>().Where(e => e.Clients.Select(c => c.ClientId).Contains(client.ClientId)).Select(e => e.EthnicityId).ToList();
                
                var demographicInfoViewModel = new DemographicInfoViewModel
                                               {
                                                   TotalAdults = client.NumberOfAdults,
                                                   ClientId = client.ClientId,
                                                   ClientFirstName = client.FirstName,
                                                   ClientLastName = client.LastName,
                                                   ResidentIncomes = incomes,
                                                   Ethnicities = ethnicities,
                                               };

                demographicInfoViewModel.InjectFrom(client);


                return View(demographicInfoViewModel);
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
        [UsesEthnicities]
        [UsesOtherReferrals]
        public ActionResult DemographicInfo(DemographicInfoViewModel demographicInfoViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var client = _repository.Single<Client>(c => c.ClientId == demographicInfoViewModel.ClientId);
                    
                    client.PartneringOrganization = demographicInfoViewModel.PartneringOrganization;
                    client.OtherReferrals = demographicInfoViewModel.OtherReferrals;
                    client.Department = demographicInfoViewModel.Department;
                    client.CaseworkerName = demographicInfoViewModel.CaseworkerName;
                    client.CaseworkerPhoneNumber = (!string.IsNullOrEmpty(demographicInfoViewModel.CaseworkerPhoneNumber))
                                                       ? Regex.Replace(demographicInfoViewModel.CaseworkerPhoneNumber, @"\D", string.Empty)
                                                       : null;
                    client.CaseworkerEmailAddress = demographicInfoViewModel.CaseworkerEmailAddress;
                    client.IsReplacingFurniture = demographicInfoViewModel.IsReplacingFurniture;
                    client.FiledFederalIncomeTax = demographicInfoViewModel.FiledFederalIncomeTax;
                    client.AgeRange = demographicInfoViewModel.AgeRange;
                    client.SchoolLevel = demographicInfoViewModel.SchoolLevel;
                    client.HasDisability = demographicInfoViewModel.HasDisability;

                    // Ethnicities
                    client.Ethnicities.Clear();
                    var ethnicities = _repository.All<Ethnicity>();
                    if (client.Ethnicities == null)
                    {
                        client.Ethnicities = new List<Ethnicity>();
                    }
                    if (demographicInfoViewModel.Ethnicities != null)
                    {
                        foreach (int ethnicityId in demographicInfoViewModel.Ethnicities)
                        {
                            client.Ethnicities.Add(ethnicities.Single(e => e.EthnicityId == ethnicityId));
                        }
                    }

                    // Incomes
                    if (demographicInfoViewModel.ResidentIncomes != null && demographicInfoViewModel.ResidentIncomes.Count > 0)
                    {
                        var existingIncomeLines = _repository.All<ResidentIncome>().Where(i => i.ClientId == demographicInfoViewModel.ClientId);
                        foreach (var income in demographicInfoViewModel.ResidentIncomes)
                        {
                            if (!string.IsNullOrEmpty(income.ResidentName))
                            {
                                var existingIncomeLine = existingIncomeLines.SingleOrDefault(i => i.ResidentIncomeId == income.ResidentIncomeId);
                                if (existingIncomeLine != null)
                                {
                                    existingIncomeLine.IsHeadOfHousehold = income.IsHeadOfHousehold;
                                    existingIncomeLine.ResidentName = income.ResidentName;
                                    existingIncomeLine.Relationship = income.Relationship;
                                    existingIncomeLine.MonthlyIncome = income.MonthlyIncome;
                                    existingIncomeLine.SourceOfIncome = income.SourceOfIncome;
                                }
                                else
                                {
                                    client.ResidentIncomes.Add(income);
                                }
                            }
                        }
                    }

                    client.LastUpdatedDateTime = DateTime.UtcNow;

                    _repository.CommitChanges();

                    this.FlashSuccess(string.Format("The client record for {0} {1} has been updated successfully.",
                                                    client.FirstName, client.LastName));
                    return RedirectToAction("DemographicInfo", new {id = client.ClientId});
                }
            }
            catch (Exception ex)
            {
                this.FlashError("Unable to save changes to client record.");
                _logger.Error(ex);

                if (demographicInfoViewModel.Ethnicities == null)
                {
                    demographicInfoViewModel.Ethnicities = _repository.All<Ethnicity>().Where(e => e.Clients.Select(c => c.ClientId).Contains(demographicInfoViewModel.ClientId)).Select(e => e.EthnicityId).ToList();
                }
            }

            return View(demographicInfoViewModel);
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
                return RedirectToAction("BasicInfo", new {id});
            }
        }

        public ActionResult Items(int id)
        {
            var rivm = new RequestedItemsViewModel();
            var client = _repository.Single<Client>(c => c.ClientId == id);
            if (client != null)
            {
                rivm.InjectFrom(client);
                var appt = _repository.Single<Appointment>(a => a.ClientId == id && a.AppointmentStatusId != (byte) Constants.AppointmentStatusId.Closed);
                if (appt != null)
                {
                    rivm.InjectFrom(appt);
                    var requestedItems = _repository.All<RequestedItem>().Where(r => r.AppointmentId == appt.AppointmentId).OrderByDescending(r => r.QuantityReceived).ThenBy(r => r.ItemName);
                    rivm.RequestedItems = new List<RequestedItem>();
                    foreach (var item in requestedItems)
                    {
                        rivm.RequestedItems.Add(item);
                    }
                }
            }

            return View(rivm);
        }

        [HttpPost]
        public ActionResult Items(int clientId, int appointmentId, IEnumerable<RequestedItem> requestedItems)
        {
            // Clear existing items
            var existingItems = _repository.All<RequestedItem>().Where(r => r.AppointmentId == appointmentId);
            foreach (var item in existingItems)
            {
                _repository.Delete(item);
            }

            foreach(var item in requestedItems)
            {
                item.AppointmentId = appointmentId;
                item.IsActive = true;
                _repository.Add(item);
            }

            _repository.CommitChanges();

            this.FlashSuccess("The Requested Items have been Saved Successfully.");
            return RedirectToAction("Items", new {id = clientId});
        }

        public ActionResult ClientReceipt(int id)
        {
            var rivm = new RequestedItemsViewModel();
            var client = _repository.Single<Client>(c => c.ClientId == id);
            if (client != null)
            {
                rivm.InjectFrom(client);
                var appt = _repository.Single<Appointment>(a => a.ClientId == id && a.AppointmentStatusId != (byte)Constants.AppointmentStatusId.Closed);
                if (appt != null)
                {
                    rivm.InjectFrom(appt);
                    var requestedItems = _repository.All<RequestedItem>().Where(r => r.AppointmentId == appt.AppointmentId).OrderByDescending(r => r.QuantityReceived).ThenBy(r => r.ItemName);
                    rivm.RequestedItems = new List<RequestedItem>();
                    foreach (var item in requestedItems)
                    {
                        rivm.RequestedItems.Add(item);
                    }
                }
            }

            return View(rivm);
        }

        [HttpPost]
        public ActionResult ClientReceipt(RequestedItemsViewModel rivm)
        {
            try
            {
                if (rivm.RequestedItems != null)
                {
                    foreach (var item in rivm.RequestedItems)
                    {
                        var existingItem = _repository.Single<RequestedItem>(i => i.RequestedItemId == item.RequestedItemId);
                        existingItem.QuantityReceived = item.QuantityReceived;
                        existingItem.ReasonForNonReceipt = item.ReasonForNonReceipt;
                    }
                }
                _repository.CommitChanges();
                this.FlashSuccess(string.Format("The client record for {0} {1} has been updated successfully.",
                                                rivm.FirstName, rivm.LastName));
                return RedirectToAction("ClientReceipt", new { id = rivm.ClientId });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                this.FlashError("Error while trying to update Requested Items for client.");
            }
            
            return View(rivm);
        }

        // Child Actions / Ajax Actions

        [ChildActionOnly]
        public ActionResult ClientNotes(int id, string refAction)
        {
            var clientNotesViewModel = new ClientNotesViewModel
            {
                ClientId = id,
                ClientNotes = _repository.All<ClientNote>().Where(c => c.ClientId == id).OrderByDescending(n => n.PostedDateTime).ToList(),
                RefAction = refAction
            };

            return View(clientNotesViewModel);
        }

        [ChildActionOnly]
        public ActionResult AppointmentQuickView(int id)
        {
            var vm = new AppointmentQuickViewModel {ClientId = id};
            var appts = _repository.All<Appointment>().Where(a => a.ClientId == id).OrderByDescending(a => a.ScheduledDateTime);
            var appt =
                appts.First(a => a.AppointmentStatusId == (byte) Constants.AppointmentStatusId.Scheduled ||
                                 a.AppointmentStatusId == (byte) Constants.AppointmentStatusId.NotScheduled);

            if (appt != null)
            {
                vm.AppointmentStatusId = appt.AppointmentStatusId;
                vm.AppointmentId = appt.AppointmentId;
                vm.CreatedDateTime = appt.CreatedDateTime;
                vm.ScheduledDateTime = (appt.ScheduledDateTime.HasValue) ? TimeHelper.ConvertToLocal(appt.ScheduledDateTime.Value) : new DateTime?();
                vm.SentLetterOrEmail = appt.SentLetterOrEmail;
                vm.TwoDayConfirmation = appt.TwoDayConfirmation;
                vm.TwoWeekConfirmation = appt.TwoWeekConfirmation;
                vm.OldAppointments = new List<AppointmentQuickViewModel.OldAppointment>();
            }

            foreach (var appointment in appts.Where(a => a.AppointmentId != appt.AppointmentId))
            {
                if (appointment.ScheduledDateTime.HasValue)
                {
                    vm.OldAppointments.Add(new AppointmentQuickViewModel.OldAppointment
                    {
                        ScheduledDateTime = TimeHelper.ConvertToLocal(appointment.ScheduledDateTime.Value),
                        Status = appointment.AppointmentStatus.Name
                    });
                }
            }

            return View(vm);
        }

        public ActionResult RequestedItemTemplate(int id)
        {
            return View("EditRequestedItem", new RequestedItem{QuantityRequested = 1});
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
            refAction = (refAction == "DemographicInfo") ? "DemographicInfo" : "BasicInfo";
            return RedirectToAction(refAction, new { id = note.ClientId });
        }
    
        [HttpPost]
        public ActionResult AddNewItem()
        {
            return PartialView("EditRequestedItem", new RequestedItem());
        }
    
        [HttpPost]
        public JsonResult CompletedTwoWeekConfirmation(int apptId)
        {
            var appt = _repository.Single<Appointment>(a => a.AppointmentId == apptId);
            if (appt != null)
            {
                appt.TwoWeekConfirmation = DateTime.UtcNow;
                _repository.CommitChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult CompletedTwoDayConfirmation(int apptId)
        {
            var appt = _repository.Single<Appointment>(a => a.AppointmentId == apptId);
            if (appt != null)
            {
                appt.TwoDayConfirmation = DateTime.UtcNow;
                _repository.CommitChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult SentMailing(int apptId)
        {
            var appt = _repository.Single<Appointment>(a => a.AppointmentId == apptId);
            if (appt != null)
            {
                appt.SentLetterOrEmail = true;
                _repository.CommitChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
    }
}
