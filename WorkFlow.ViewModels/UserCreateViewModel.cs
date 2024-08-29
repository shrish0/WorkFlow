using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WorkFlow.ViewModels
{
    public class UserCreateViewModel
    {
        public int? Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Phone]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

        public string Role { get; set; }
        public List<SelectListItem>? RoleList { get; set; } // Ensure this is populated in your controller

        public string ClearanceLevel { get; set; }
    }
}
