using System;
using System.Collections.Generic;
using AWC.Domain.Entities;

namespace AWC.WebUI.Models
{
    public class WaitListViewModel
    {
        public IEnumerable<WaitlistClient> Clients { get; set; }
    }

    public class WaitlistClient
    {
        public int ClientId { get; set; }

        public int AppointmentId { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PrimaryPhoneNumber { get; set; }

        public byte PhoneNumberTypeId { get; set; }

        public List<RequestedItem> RequestedItems { get; set; }
    }
}