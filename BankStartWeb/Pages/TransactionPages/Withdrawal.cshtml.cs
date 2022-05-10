using BankStartWeb.Data;
using BankStartWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace BankStartWeb.Pages.Transactions
{

    [BindProperties]
    [Authorize(Roles = "Admin,Cashier")]
    public class WithdrawalModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransactionServices _services;
        private readonly IToastNotification _toastNotification;

        public WithdrawalModel(ApplicationDbContext context,ITransactionServices services, IToastNotification toastNotification)
        {
            _context = context;
            _services = services;
            _toastNotification = toastNotification;
        }

        public int AccountId { get; set; }
        public int CustomerId { get; set; }
        public string Operation { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public Account Account { get; set; }
        public Customer Customer { get; set; }
        public List<SelectListItem> AllTypes { get; set; }
        public List<SelectListItem> AllOperations { get; set; }

        public void OnGet(int accountId, int customerId)
        {
            AccountId = accountId;
            CustomerId = customerId;
            
        }
        public IActionResult OnPost(int accountId, int customerId)
        {
            if (ModelState.IsValid)
            {
                Customer = _context.Customers.First(c => c.Id == customerId);
                Account = _context.Accounts.Include(t => t.Transactions).First(a => a.Id == accountId);
                var status = _services.Withdrawal(accountId, Amount);
                switch (status)
                {
                    case ITransactionServices.Status.InsufficientFunds:
                        ModelState.AddModelError(nameof(Amount),
                            "Insufficient funds");
                        return Page();
                    case ITransactionServices.Status.MinimumWithdrawal:
                        ModelState.AddModelError(nameof(Amount),
                            "Please enter an amount of 100 or more");
                        return Page();
                    case ITransactionServices.Status.LowerThanZero:
                        ModelState.AddModelError(nameof(Amount),
                            "Cannot withdrawal negative amounts.");
                        return Page();
                }
                _toastNotification.AddSuccessToastMessage("Successful Withdrawal");
                return RedirectToPage("/CustomerPages/Customer", new { customerId });
            }
            return Page();
        }
    }
}