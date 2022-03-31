using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankStartWeb.Pages
{
    public class TransactionsModel : PageModel
    {
        private readonly ApplicationDbContext context;

        public TransactionsModel(ApplicationDbContext _context)
        {
            context = _context;
        }
        public class TransactionsViewModel
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public string Operation { get; set; }
            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
            public decimal NewBalance { get; set; }
        }
        public int CustomerReference { get; set; }

        public Account Account { get; set; }
        public Customer Customer { get; set; }
        public List<TransactionsViewModel> Transactions { get; set; } 
        public void OnGet(int accountId, int customerId)
        {
            
            Account = context.Accounts.Include(t => t.Transactions).First(a => a.Id == accountId);
            Transactions = Account.Transactions.Select(t => new TransactionsViewModel
            {
                Id = accountId,
                Type = t.Type,
                Operation = t.Operation,
                Amount = t.Amount,
                Date = t.Date,
                NewBalance = t.NewBalance


            }).ToList().OrderByDescending(x => x.Date).ToList();
            CustomerReference = customerId;
        }

        public IActionResult OnPostCustomer(int customerId)
        {
            Customer = context.Customers.Include(a => a.Accounts).First(c => c.Id == customerId);

            return RedirectToPage("CustomerPages/Customer", new {customerId});
        }
    }
}
