using System.ComponentModel.DataAnnotations;

namespace AWC.Domain.Entities
{
    public class RequestedItem
    {
        [Key]
        public int RequestedItemId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string ItemName { get; set; }

        [Required]
        public short QuantityRequested { get; set; }

        [Required]
        public bool Received { get; set; }

        [Required]
        public short QuantityReceived { get; set; }

        [StringLength(1000)]
        public string ReasonForNonReceipt { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public virtual Appointment Appointment { get; set; }
    }
}
