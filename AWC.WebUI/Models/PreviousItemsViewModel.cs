using System;
using System.Collections.Generic;
using AWC.Domain.Entities;

namespace AWC.WebUI.Models
{
    public class PreviousItemsViewModel
    {
        public DateTime AppointmentDateTime { get; set; }
        public List<RequestedItem> RequestedItems { get; set; }
    }
}