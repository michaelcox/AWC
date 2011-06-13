using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace AWC.Domain.Entities
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        public DateTime ScheduledDateTime { get; set; }

        [Timestamp]
        public byte[] TimeStamp { get; set; }

        public DateTime? TwoWeekConfirmation { get; set; }

        public DateTime? TwoDayConfirmation { get; set; }

        [Required]
        public bool SentLetterOrEmail { get; set; }

        [Required]
        public bool RequestFulfilled { get; set; }

        [Required]
        public byte StatusId { get; set; }

        public int ClientId { get; set; }

        public virtual Client Client { get; set; }
    }
}
