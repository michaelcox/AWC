using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AWC.Domain.Entities
{
    public class County
    {
        [Key]
        [StringLength(2)]
        public string CountyCode { get; set; }

        [StringLength(100)]
        public string CountyName { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
    }
}
