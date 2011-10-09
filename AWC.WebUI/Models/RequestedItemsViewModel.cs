using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AWC.Domain.Entities;

namespace AWC.WebUI.Models
{
    public class RequestedItemsViewModel
    {
        [Required]
        public int RequestedItemId { get; set; }

        [Required]
        public int ClientId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Required]
        [Display(Name = "Quantity Requested")]
        public short QuantityRequested { get; set; }

        [Required]
        [Display(Name = "Quantity Received")]
        public short QuantityReceived { get; set; }

        [StringLength(1000)]
        [Display(Name = "Reason Item Not Received")]
        public string ReasonForNonReceipt { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public List<RequestedItem> RequestedItems { get; set; }
    }
}