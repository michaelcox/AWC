using System;
using System.ComponentModel.DataAnnotations;

namespace AWC.Domain.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(50)]
        public string EmailAddress { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(50)]
        public string Password { get; set; }
        
        [Required]
        public DateTime CreationDate { get; set; }
        
        public DateTime? LastLoginDate { get; set; }
        
        [Required]
        public bool IsActive { get; set; }
    }
}
