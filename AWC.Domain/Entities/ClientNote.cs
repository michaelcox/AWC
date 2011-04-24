using System.ComponentModel.DataAnnotations;

namespace AWC.Domain.Entities
{
    public class ClientNote
    {
        [Key]
        public int ClientNoteId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Timestamp]
        public byte[] TimeStamp { get; set; }

        [StringLength(4000)]
        [Required(AllowEmptyStrings = false)]
        public string Body { get; set; }

        public virtual Client Client { get; set; }
    }
}
