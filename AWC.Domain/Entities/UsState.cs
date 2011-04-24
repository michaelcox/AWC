﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AWC.Domain.Entities
{
    public class UsState
    {
        [Key]
        [StringLength(2)]
        public string StateCode { get; set; }
        
        [StringLength(20)]
        public string StateName { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
    }
}
