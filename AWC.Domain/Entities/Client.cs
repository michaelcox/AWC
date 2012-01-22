using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AWC.Domain.Entities
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [Required]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        public DateTime LastUpdatedDateTime { get; set; }

        [Timestamp]
        public byte[] TimeStamp { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(20)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(20)]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(10)]
        public string PrimaryPhoneNumber { get; set; }

        [Required]
        public byte PrimaryPhoneTypeId { get; set; }

        [StringLength(10)]
        public string SecondaryPhoneNumber { get; set; }

        public byte SecondaryPhoneTypeId { get; set; }

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

        [Required]
        public bool IsReplacingFurniture { get; set; }

        public int? CaseworkerId { get; set; }
        
        // Virtual / Foreign Keys
        public virtual Caseworker Caseworker { get; set; }

        public virtual ICollection<ClientNote> ClientNotes { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }

        public virtual ICollection<ResidentIncome> ResidentIncomes { get; set; } 

        public virtual UsState UsState { get; set; }
        
        public virtual County County { get; set; }

    }
}
