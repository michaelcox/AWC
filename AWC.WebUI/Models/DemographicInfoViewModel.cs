﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AWC.Domain.Entities;
using AWC.Domain.Metadata;
using System.Web.Mvc;

namespace AWC.WebUI.Models
{
    public class DemographicInfoViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int CaseworkerId { get; set; }

        [Required]
        [HiddenInput(DisplayValue = false)]
        public int ClientId { get; set; }

        [StringLength(50)]
        [Display(Name = "Partnering Organization")]
        public string PartneringOrganization { get; set; }

        [StringLength(50)]
        [Display(Name = "Caseworker Name")]
        public string Name { get; set; }

        [StringLength(10)]
        [Display(Name = "Caseworker Phone Number")]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        [Display(Name = "Caseworker Email Address")]
        public string Email { get; set; }

        [StringLength(50)]
        [Display(Name = "Department / Division")]
        public string Department { get; set; }

        [Display(Name = "Is the client replacing furniture?")]
        [Option(DisplayText = "Yes", Value = true)]
        [Option(DisplayText = "No", Value = false)]
        public bool IsReplacingFurniture { get; set; }

        public List<ResidentIncome> ResidentIncomes { get; set; }

        [Display(Name = "Did client file a federal income tax form for the most recent year?")]
        [Option(DisplayText = "Yes", Value = true)]
        [Option(DisplayText = "No", Value = false)]
        public bool FiledFederalIncomeTax { get; set; }

        public List<int> Ethnicities { get; set; } 

        public string AgeRange { get; set; }

        [Display(Name = "Does the client have a disability?")]
        [Option(DisplayText = "Yes", Value = true)]
        [Option(DisplayText = "No", Value = false)]
        public bool HasDisability { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ClientFirstName { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ClientLastName { get; set; }

        [HiddenInput(DisplayValue = false)]
        public short TotalAdults { get; set; }
    }
}