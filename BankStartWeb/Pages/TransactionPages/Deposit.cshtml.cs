using System.ComponentModel.DataAnnotations;
using BankStartWeb.Data;
using BankStartWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BankStartWeb.Pages.Transactions
{
    [BindProperties]
    [Authorize(Roles = "Admin,Cashier")]
    public class DepositModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransactionServices _services;

        public DepositModel(ApplicationDbContext context, ITransactionServices services)
        {
            _context = context;
            _services = services;
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
                        "Enter amount more than 0");
                    return Page();
                }
                //Account.Balance += Amount;
                
                return RedirectToPage("/CustomerPages/Customer", new { customerId });
            }
            return Page();
        }
        
    }
}