using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.Models;

namespace WorkFlow.ViewModels
{
    public class UserListViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public string CurrentUserId { get; set; }
    }

}
