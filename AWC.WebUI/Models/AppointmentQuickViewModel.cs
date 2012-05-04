using System;
using System.Collections.Generic;

namespace AWC.WebUI.Models
{
    public class AppointmentQuickViewModel
    {
        public int ClientId { get; set; }
        public int? AppointmentId { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? ScheduledDateTime { get; set; }
        public DateTime? TwoWeekConfirmation { get; set; }
        public DateTime? TwoDayConfirmation { get; set; }
        public bool SentLetterOrEmail { get; set; }
        public byte? AppointmentStatusId { get; set; }
        public List<OldAppointment> OldAppointments { get; set; }

        public class OldAppointment
        {
            public DateTime ScheduledDateTime { get; set; }
            public string Status { get; set; }
        }
    }
}