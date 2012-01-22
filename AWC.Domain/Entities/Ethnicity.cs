using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AWC.Domain.Entities
{
    public class Ethnicity
    {
        [Key]
        public int EthnicityId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<Client> Clients { get; set; } 
    }
}
