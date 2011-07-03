using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace AWC.Domain.Entities
{
    public class AppointmentStatus
    {
        [Key]
        public byte AppointmentStatusId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}
