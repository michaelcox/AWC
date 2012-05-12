using System.Collections.Generic;
using AWC.Domain.Entities;

namespace AWC.WebUI.Models
{
    public class RequestedItemsViewModel
    {
        public int RequestedItemId { get; set; }

        public int ClientId { get; set; }

        public int AppointmentId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<RequestedItem> RequestedItems { get; set; }

        public List<PreviousItemsViewModel> PreviousItems { get; set; } 
    }
}