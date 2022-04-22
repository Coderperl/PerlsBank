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
    public class WithdrawalModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransactionServices _services;

        public WithdrawalModel(ApplicationDbContext context,ITransactionServices services)
        {
            _context = context;
            _services = services;
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
                //var withdrawal = new Transaction
                //{
                //    Amount = Amount,
                //    Operation = Operation,
                //    Date = DateTime.Now,
                //    Type = Type,
                //    NewBalance = Account.Balance - Amount
                //};
                var status = _services.Withdrawal(accountId, Amount);
                if (status == ITransactionServices.Status.InsufficientFunds)
                {
                    ModelState.AddModelError(nameof(Amount),
                        "Insufficient funds");
                    
                    return Page();
                }
                if (Amount < 100)
                {
                    ModelState.AddModelError(nameof(Amount),
                        "Please enter an amount of 100 or more");
                    
                    return Page();
                }

                
                Account.Balance -= Amount;
                _context.SaveChanges();
                return RedirectToPage("/CustomerPages/Customer", new { customerId });
            }
           
            return Page();

        }

       
    }
}