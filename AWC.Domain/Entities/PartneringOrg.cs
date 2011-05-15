using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AWC.Domain.Entities
{
    public class PartneringOrg
    {
        [Key]
        public int PartneringOrgId { get; set; }

        [StringLength(100)]
        [Required]
        public string OrganizationName { get; set; }

        public virtual ICollection<Caseworker> Caseworkers { get; set; }
    }
}
