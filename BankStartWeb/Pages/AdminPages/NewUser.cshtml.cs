using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankStartWeb.Pages.AdminPages
{[BindProperties]
    public class NewUserModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public NewUserModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public string UserName { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool EmailConfirmed { get; set; }
        public IList<string> Roles { get; set; }


        public List<SelectListItem> AllRoles { get; set; }
        public void OnGet()
        {
            SetRoles();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                
                var user = new IdentityUser();
                {
                    user.UserName = UserName;
                    user.Email = Email;
                    user.PasswordHash = Password;
                    EmailConfirmed = true;
                };
                
                _userManager.CreateAsync(user, Password).Wait();
                _userManager.AddToRolesAsync(user, Roles).Wait();
                return RedirectToPage("/AdminPages/SystemUsers");
            }
            
            return Page();
        }

        public void SetRoles()
        {
            AllRoles = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Value = "Cashier",
                    Text = "Cashier"
                },
                new SelectListItem()
                {
                    Value ="Admin",
                    Text = "Admin"
                }
            };
        }
    }
}
