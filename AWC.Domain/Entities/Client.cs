using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AWC.Domain.Entities
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(20)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(20)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string EmailAddress { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        public string AddressLine1 { get; set; }

        [StringLength(50)]
        public string AddressLine2 { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(2)]
        public string StateCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(2)]
        public string CountyCode { get; set; }

        public bool IsPreviousClient { get; set; }

        public short NumberOfAdults { get; set; }

        public short NumberOfChildren { get; set; }

        [StringLength(100)]
        public string ReferredFrom { get; set; }

        public virtual ICollection<ClientNote> ClientNotes { get; set; }
        public virtual ICollection<RequestedItem> RequestedItems { get; set; }

        public virtual UsState UsState { get; set; }
        public virtual County County { get; set; }
    }
}
