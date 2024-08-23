using System.ComponentModel.DataAnnotations;

namespace WorkFlow.ViewModels
{
   public class UserViewModel
    {
        public string Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public string CreatedBy { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; } // Added LockoutEnd property
    }

}
