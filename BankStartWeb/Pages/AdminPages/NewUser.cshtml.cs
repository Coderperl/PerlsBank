using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankStartWeb.Pages.AdminPages
{
    [Authorize(Roles = "Admin")]
    public class NewUserModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public NewUserModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        [Required(ErrorMessage = "Please enter a username that is the same as Email")]
        public string UserName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter an Email-address")]
        public string Email { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter a password with atleast 8 characters.")]
        public string Password { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Please enter a role.")]
        public IList<string> Roles { get; set; }


        public List<SelectListItem> AllRoles { get; set; }

        public void OnGet()
        {
            SetRoles();
        }

        public IActionResult OnPost()
        {
            if (Roles== null)
            {
                ModelState.AddModelError(nameof(Roles), "Please select atleast one role.");
                SetRoles();
                return Page();
            }

            if (ModelState.IsValid)
            {
                var user = new IdentityUser();
                {
                    user.UserName = UserName;
                    user.Email = Email;
                    user.PasswordHash = Password;
                    user.EmailConfirmed = true;
                }
                ;
                _userManager.CreateAsync(user, Password).Wait();
                _userManager.AddToRolesAsync(user, Roles).Wait();
                return RedirectToPage("/AdminPages/SystemUsers");
            }

            SetRoles();
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
                    Value = "Admin",
                    Text = "Admin"
                }
            };
        }
    }
}