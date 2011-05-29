using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using AWC.Domain.Metadata;
using System.Web.Mvc;

namespace AWC.WebUI.Models
{
    public class PartnerInfoViewModel
    {
        [Required]
        [HiddenInput(DisplayValue = false)]
        public int CaseworkerId { get; set; }

        [Required]
        [HiddenInput(DisplayValue = false)]
        public int ClientId { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Caseworker First Name")]
        public string FirstName { get; set; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Caseworker Last Name")]
        public string LastName { get; set; }

        [StringLength(50)]
        [Display(Name = "Caseworker #")]
        public string ExternalId { get; set; }

        [StringLength(50)]
        [Display(Name = "Caseworker Email Address")]
        public string Email { get; set; }

        [StringLength(100)]
        [Display(Name = "Department / Division")]
        public string Department { get; set; }

        [Required]
        [Display(Name = "Partnering Organization")]
        public int PartneringOrgId { get; set; }

        [Required]
        [Display(Name = "Are you replacing furniture?")]
        [Option(DisplayText = "Yes", Value = true)]
        [Option(DisplayText = "No", Value = false)]
        public bool IsReplacingFurniture { get; set; }

        public ClientNotesViewModel ClientNotesViewModel { get; set; }

        [ScaffoldColumn(false)]
        public string ClientFirstName { get; set; }

        [ScaffoldColumn(false)]
        public string ClientLastName { get; set; }
    }
}