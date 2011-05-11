using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AWC.Domain.Entities;

namespace AWC.WebUI.Models
{
    public class RequestedItemsViewModel
    {
        [Key]
        public int RequestedItemId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        [Display(Name = "Item Name")]
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

        public List<RequestedItem> RequestedItems { get; set; }
    }
}