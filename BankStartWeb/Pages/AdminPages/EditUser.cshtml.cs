using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BankStartWeb.Pages.AdminPages;

[BindProperties]
public class EditUserModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public EditUserModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public string UserId { get; set; }

    [Required(ErrorMessage = "Please enter a username that is the same as Email")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Please enter an Email-address")]
    public string Email { get; set; }
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Please enter a password with atleast 8 characters.")]
    public string Password { get; set; }
    [Required(ErrorMessage = "Please select atleast one role.")]
    public IList<string> Roles { get; set; }
    public List<SelectListItem> AllRoles { get; set; }

    public void OnGet(string userId)
    {
        UserId = userId;
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
        UserName = user.UserName;
        Email = user.Email;
        Password = user.PasswordHash;
        SetRoles();
    }

    public IActionResult OnPost(string userId)
    {
        if (Roles.Count == 0)
        {
            ModelState.AddModelError(nameof(Roles),"Please select atleast one role.");
            SetRoles();
            return Page();
        }
        if(ModelState.IsValid)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            {
                var roles = _userManager.GetRolesAsync(user).Result;
                _userManager.RemoveFromRolesAsync(user, roles).Wait();
                _userManager.UpdateAsync(user).Wait();
                user.UserName = UserName;
                user.Email = Email;
                Password = user.PasswordHash;
                _userManager.AddToRolesAsync(user, Roles).Wait();
                _userManager.UpdateAsync(user).Wait();
            }
            return RedirectToPage("/AdminPages/SystemUsers");
        }
        return Page();
    }

    public void SetRoles()
    {
        AllRoles = new List<SelectListItem>
        {
            new()
            {
                Value = "Cashier",
                Text = "Cashier"
            },
            new()
            {
                Value = "Admin",
                Text = "Admin"
            }
        };
    }
}