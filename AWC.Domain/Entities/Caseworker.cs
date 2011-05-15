using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AWC.Domain.Entities
{
    public class Caseworker
    {
        [Key]
        public int CaseworkerId { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string ExternalId { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Department { get; set; }

        [Required]
        public int PartneringOrgId { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        public virtual PartneringOrg ParneringOrg { get; set; }
    }
}
