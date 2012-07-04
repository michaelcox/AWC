using System;
using System.Collections.Generic;
using System.Data;
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
        public ActionResult CountyDistribution(int? month, int? year, string countyCode, bool? download)
        {
            var report = new List<CountyDistributionReportViewModel>();

            if (month.HasValue && month.Value > 0 && year.HasValue && year.Value >= 2012 && !string.IsNullOrEmpty(countyCode))
            {
                report = GetCountyDistributionReport(month.Value, year.Value, countyCode);

                if (download.HasValue && download.Value == true)
                {
                    // Turn it into a datatable, so it can be downloaded as a CSV
                    var dt = new DataTable();
                    dt.Columns.Add(new DataColumn("Date"));
                    dt.Columns.Add(new DataColumn("FirstName"));
                    dt.Columns.Add(new DataColumn("LastName"));
                    dt.Columns.Add(new DataColumn("NumberOfAdults"));
                    dt.Columns.Add(new DataColumn("NumberOfChildren"));
                    dt.Columns.Add(new DataColumn("City"));
                    dt.Columns.Add(new DataColumn("State"));
                    dt.Columns.Add(new DataColumn("ReceivedItems"));

                    foreach (var vm in report)
                    {
                        var row = dt.NewRow();
                        row["Date"] = vm.ReceivedDateTime.ToShortDateString();
                        row["FirstName"] = vm.FirstName;
                        row["LastName"] = vm.LastName;
                        row["NumberOfAdults"] = vm.NumberOfAdults;
                        row["NumberOfChildren"] = vm.NumberOfChildren;
                        row["City"] = vm.City;
                        row["State"] = vm.StateCode;
                        row["ReceivedItems"] = vm.ReceivedItems;
                        dt.Rows.Add(row);
                    }

                    return new CsvActionResult(dt)
                    {
                        FileDownloadName = ("awc-countydist-" + countyCode + "-" + month.Value + "-" + year.Value + ".csv").ToLower()
                    };
                }
            }

            return View(report);
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        private List<CountyDistributionReportViewModel> GetCountyDistributionReport(int month, int year, string countyCode)
        {
            var report = new List<CountyDistributionReportViewModel>();

            if (month > 0 && year >= 2012 && !string.IsNullOrEmpty(countyCode))
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
                                vm.ReceivedItems = vm.ReceivedItems.TrimEnd(',', ' ');

                                // Only add people to the report who have received items
                                report.Add(vm);
                            }
                        }
                    }
                }

            }

            return report;
        }
    
    }
}
