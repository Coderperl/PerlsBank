using System.ComponentModel.DataAnnotations;
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
    public class DepositModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransactionServices _services;
        private readonly IToastNotification _toastNotification;

        public DepositModel(ApplicationDbContext context, ITransactionServices services, IToastNotification toastNotification)
        {
            _context = context;
            _services = services;
            _toastNotification = toastNotification;
        }

        public string Operation { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public Customer Customer { get; set; }
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
                var status = _services.Deposit(accountId,Amount);
                if (status == ITransactionServices.Status.LowerThanZero)
                {
                    ModelState.AddModelError(nameof(Amount),
                        "Cannot deposit negative amounts");
                    return Page();
                }
                _toastNotification.AddSuccessToastMessage("Successful deposit");
                return RedirectToPage("/CustomerPages/Customer", new { customerId });
            }
            return Page();
        }
    }
}