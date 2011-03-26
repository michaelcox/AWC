using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace AWC.Domain.Entities
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required.")]
        [MaxLength(20, ErrorMessage = "First Name is too long.")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required.")]
        [MaxLength(20, ErrorMessage = "Last Name is too long.")]
        public string LastName { get; set; }

        [Email(ErrorMessage = "Email address is not valid.")]
        public string EmailAddress { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Address is required.")]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "State is required.")]
        [MaxLength(2)]
        [MinLength(2)]
        public string StateCode { get; set; }

        public string CountyCode { get; set; }

        public bool IsPreviousClient { get; set; }

        [Integer]
        [Max(20, ErrorMessage = "There are too many adults specified.")]
        public short NumberOfAdults { get; set; }

        [Integer]
        [Max(20, ErrorMessage = "There are too many children specified.")]
        public short NumberOfChildren { get; set; }

        public string ReferredFrom { get; set; }
    }
}
