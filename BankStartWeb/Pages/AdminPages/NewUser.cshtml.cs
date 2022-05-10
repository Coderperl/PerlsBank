using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;

namespace BankStartWeb.Pages.AdminPages
{
    [Authorize(Roles = "Admin")]
    public class NewUserModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IToastNotification _toastNotification;

        public NewUserModel(UserManager<IdentityUser> userManager, IToastNotification toastNotification)
        {
            _userManager = userManager;
            _toastNotification = toastNotification;
        }

        [BindProperty]
        [Required(ErrorMessage = "Please enter a username that is the same as Email")]
        public string UserName { get; set; }

        [BindProperty]
        [EmailAddress]
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
                var result = _userManager.CreateAsync(user, Password).Result;
                
                if (result.Succeeded)
                {
                    _userManager.AddToRolesAsync(user, Roles).Wait();
                }
                else
                {
                    ModelState.AddModelError(nameof(Password), "Please enter a password that starts with a capital letter");
                    SetRoles();
                    return Page();
                }
                _toastNotification.AddSuccessToastMessage("New user added.");
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