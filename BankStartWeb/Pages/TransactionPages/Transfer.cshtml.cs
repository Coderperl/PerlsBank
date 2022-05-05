using System.ComponentModel.DataAnnotations;
using BankStartWeb.Data;
using BankStartWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BankStartWeb.Pages.TransactionPages
{   
    [BindProperties]
    [Authorize(Roles = "Admin,Cashier")]
    public class TransferModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransactionServices _services;

        public TransferModel(ApplicationDbContext context, ITransactionServices services)
        {
            _context = context;
            _services = services;
        }
        public int AccountId { get; set; }
        [Range(1,Int16.MaxValue, ErrorMessage = "Invalid account number")]
        public int TransferId { get; set; }
        public int CustomerId { get; set; }
        public string Operation { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public Customer Customer { get; set; }
        public List<Account> Accounts { get; set; }
        public List<SelectListItem> ReceivingAccounts { get; set; }
        public void OnGet(int accountId, int customerId)
        {
            Customer = _context.Customers.Include(a => a.Accounts).First(c => c.Id == customerId);
            Accounts = Customer.Accounts.Select(a => new Account 
            {
                Id = a.Id,

            }).ToList();
            AccountId = accountId;
            CustomerId = customerId;
        }

        public IActionResult OnPost(int customerId, int accountId)
        {

            if (ModelState.IsValid)
            {
                Customer = _context.Customers.First(c => c.Id == customerId);
                var status = _services.Transfer(accountId, TransferId,Amount);
                if (status == ITransactionServices.Status.InsufficientFunds)
                {
                    ModelState.AddModelError(nameof(Amount),
                        "Insufficient funds");
                    return Page();
                }
                if (status == ITransactionServices.Status.Error)
                {
                    ModelState.AddModelError(nameof(TransferId),
                        "Please select another account.");
                    return Page();
                }
                if (status == ITransactionServices.Status.LowerThanZero)
                {
                    ModelState.AddModelError(nameof(Amount),
                        "You cannot transfer a negative amount.");
                    return Page();
                }
                return RedirectToPage("/CustomerPages/Customer", new { customerId });
            }
            return Page();

        }
        
        
    }
}
