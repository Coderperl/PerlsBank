using BankStartWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankStartWeb.Pages.AdminPages
{
    [Authorize(Roles = "Admin")]
    public class SystemUsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public SystemUsersModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        
        public class UserViewModel
        {
            public string Id { get; set; }
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public IList<string> UserRoles { get; set; }
        }

        public List<UserViewModel> Users { get; set; }

        public void OnGet(string userId)
        {
            
            Users = _userManager.Users.ToList().Select(u => new UserViewModel()
            {
                UserId = userId,
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                UserRoles = _userManager.GetRolesAsync(u).Result
            }).ToList();
        }
    }
}
