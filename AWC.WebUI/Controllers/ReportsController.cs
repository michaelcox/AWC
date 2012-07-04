using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AWC.Domain.Abstract;
using AWC.Domain.Entities;
using AWC.WebUI.Helpers;
using AWC.WebUI.Infrastructure.Logging;
using AWC.WebUI.Models;

namespace AWC.WebUI.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;

        public ReportsController(IRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [UsesCountiesDropdown]
        public ActionResult CountyDistribution(int? month, int? year, string countyCode)
        {
            var report = new List<CountyDistributionReportViewModel>();

            if (month.HasValue && month.Value > 0 && year.HasValue && year.Value >= 2012 && !string.IsNullOrEmpty(countyCode))
            {
                // Get all appointments that were completed this selected month
                var appointments =
                    _repository.All<Appointment>().Where(
                        a =>
                        a.ScheduledDateTime.HasValue && a.ScheduledDateTime.Value.Month == month &&
                        a.ScheduledDateTime.Value.Year == year).ToList();

                if (appointments.Any())
                {
                    // Get all client info
                    var clientIds = appointments.Select(a => a.ClientId).Distinct().ToList();
                    var clients = _repository.All<Client>().Where(c => clientIds.Contains(c.ClientId)).ToList();

                    // Get all the requested items
                    var appoitnmentIds = appointments.Select(a => a.AppointmentId).Distinct().ToList();
                    var requestedItems = _repository.All<RequestedItem>().Where(r => appoitnmentIds.Contains(r.AppointmentId)).ToList();

                    // Loop through to create the report
                    foreach (var appointment in appointments.OrderBy(a => a.ScheduledDateTime))
                    {
                        var vm = new CountyDistributionReportViewModel();
                        var client = clients.Single(c => c.ClientId == appointment.ClientId);

                        // Make sure the client meets the search criteria
                        if (client.CountyCode == countyCode)
                        {
                            vm.ReceivedDateTime = appointment.ScheduledDateTime.Value;
                            vm.FirstName = client.FirstName;
                            vm.LastName = client.LastName;
                            vm.NumberOfAdults = client.NumberOfAdults;
                            vm.NumberOfChildren = client.NumberOfChildren;
                            vm.City = client.City;
                            vm.StateCode = client.StateCode;

                            var items =
                                requestedItems.Where(
                                    r => r.AppointmentId == appointment.AppointmentId && r.QuantityReceived > 0).ToList();
                            if (items.Any())
                            {
                                foreach (var item in items)
                                {
                                    vm.ReceivedItems += item.QuantityReceived + " " + item.ItemName + ", ";
                                }
                                vm.ReceivedItems =  vm.ReceivedItems.TrimEnd(',', ' ');

                                // Only add people to the report who have received items
                                report.Add(vm);
                            }
                        }
                    }
                }

            }

            return View(report);
        }
    }
}
