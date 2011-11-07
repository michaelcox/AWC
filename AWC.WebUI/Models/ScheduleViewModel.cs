using System;
using System.Collections.Generic;
using AWC.Domain.Entities;

namespace AWC.WebUI.Models
{
    public class ScheduleViewModel
    {
        public IEnumerable<ScheduledClient> Clients { get; set; }
    }

    public class ScheduledClient
    {
        public int ClientId { get; set; }

        public int AppointmentId { get; set; }

        public byte AppointmentStatusId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PrimaryPhoneNumber { get; set; }

        public byte PhoneNumberTypeId { get; set; }

        public DateTime ScheduledDateTime { get; set; }

        public DateTime? TwoWeekConfirmation { get; set; }

        public DateTime? TwoDayConfirmation { get; set; }

        public bool SentLetterOrEmail { get; set; }

        public IEnumerable<RequestedItem> RequestedItems { get; set; }

        public IEnumerable<ClientNote> ClientNotes { get; set; }
    }
}