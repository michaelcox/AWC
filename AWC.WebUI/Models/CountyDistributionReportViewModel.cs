using System;

namespace AWC.WebUI.Models
{
    public class CountyDistributionReportViewModel
    {
        public int ClientId { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public short NumberOfAdults { get; set; }
        public short NumberOfChildren { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string ReceivedItems { get; set; }
    }
}