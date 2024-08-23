using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WorkFlow.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Address { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        [MaxLength(10)]
        public string? CreatedBy { get; set; }
        [MaxLength(10)]
        public string ApplicationUserId { get; set; }
        [MaxLength(5)]
        public string ClearanceLevel { get; set; } = "Cl01";

        [MaxLength(10)]
        public string? BlockedBy { get; set; } 


    }
}
