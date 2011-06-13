using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AWC.Domain.Entities;

namespace AWC.WebUI.Models
{
    public class WaitListViewModel
    {
        public int ClientId { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<RequestedItem> RequestedItems { get; set; }

        public List<ClientNote> ClientNotes { get; set; }
    }
}