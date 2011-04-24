using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AWC.Domain.Entities;

namespace AWC.WebUI.Models
{
    public class ClientNotesViewModel
    {
        public List<ClientNote> ClientNotes { get; set; }

        [Required]
        public int ClientId { get; set; }

        [StringLength(4000)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a note.")]
        public string Body { get; set; }

    }
}