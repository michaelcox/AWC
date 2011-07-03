using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public DateTime CreatedDateTime { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PrimaryPhoneNumber { get; set; }

        public byte PhoneNumberTypeId { get; set; }

        public IEnumerable<RequestedItem> RequestedItems { get; set; }

        public IEnumerable<ClientNote> ClientNotes { get; set; }
    }
}