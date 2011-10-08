using System.ComponentModel.DataAnnotations;

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
