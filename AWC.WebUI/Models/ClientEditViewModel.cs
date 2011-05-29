using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using AWC.Domain.Metadata;
using DataAnnotationsExtensions;

namespace AWC.WebUI.Models
{
    public class ClientEditViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int ClientId { get; set; }

        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required.")]
        [StringLength(20, ErrorMessage = "First Name is too long.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required.")]
        [StringLength(20, ErrorMessage = "Last Name is too long.")]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        [Email(ErrorMessage = "Email address is not valid.")]
        [StringLength(50, ErrorMessage = "Email Address is too long.")]
        public string EmailAddress { get; set; }

        [Display(Name = "Address Line 1")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Address is required.")]
        [StringLength(50, ErrorMessage = "Address is too long.")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        [StringLength(50, ErrorMessage = "Address is too long.")]
        public string AddressLine2 { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the city this client lives in.")]
        [StringLength(50, ErrorMessage = "City is too long.")]
        public string City { get; set; }

        [Display(Name = "State")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the state this client lives in.")]
        [MaxLength(2)]
        [MinLength(2)]
        public string StateCode { get; set; }

        [Display(Name = "County")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the county this client lives in.")]
        public string CountyCode { get; set; }

        [Display(Name = "Are You a Previous Client?")]
        [Option(DisplayText = "No", Value = false)]
        [Option(DisplayText = "Yes", Value = true)]
        public bool IsPreviousClient { get; set; }

        [Display(Name = "Number of Adults")]
        [Integer(ErrorMessage = "Please enter a valid number.")]
        [Max(10, ErrorMessage = "There are too many adults specified.")]
        public short NumberOfAdults { get; set; }

        [Display(Name = "Number of Children")]
        [Integer(ErrorMessage = "Please enter a valid number.")]
        [Max(10, ErrorMessage = "There are too many children specified.")]
        public short NumberOfChildren { get; set; }

        [Display(Name = "How Did You Hear About Us?")]
        public string ReferredFrom { get; set; }

        public ClientNotesViewModel ClientNotesViewModel { get; set; }

        public RequestedItemsViewModel RequestedItemsViewModel { get; set; }
    }
}