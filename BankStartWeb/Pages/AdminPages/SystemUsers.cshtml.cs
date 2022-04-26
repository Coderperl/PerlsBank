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
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SystemUsersModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class UserViewModel
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public IList<string> UserRoles { get; set; }
        }

        public List<UserViewModel> Users { get; set; }
        public List<RoleViewModel> Roles { get; set; }

        public class RoleViewModel
        {
            public string RoleName { get; set; }
            
        }
        public void OnGet()
        {
            Users = _userManager.Users.Select(u => new UserViewModel()
            {
                UserName = u.UserName,
                Email = u.Email,
                UserRoles = _userManager.GetRolesAsync(u).Result
            }).ToList();
        }
    }
}
