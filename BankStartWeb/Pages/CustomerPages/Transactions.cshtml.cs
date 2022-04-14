using BankStartWeb.Data;
using BankStartWeb.Infrastructure.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankStartWeb.Pages
{
    [Authorize(Roles = "Admin,Cashier")]
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

        public int AccountId { get; set; }
        public Account Account { get; set; }
        public Customer Customer { get; set; }
        public List<TransactionsViewModel> Transactions { get; set; }
        public void OnGet(int accountId, int customerId)
        {

            Account = context.Accounts.Include(t => t.Transactions.OrderByDescending(x => x.Date)).First(a => a.Id == accountId);
            Transactions = Account.Transactions.Take(5).Select(t => new TransactionsViewModel
            {
                Id = accountId,
                Type = t.Type,
                Operation = t.Operation,
                Amount = t.Amount,
                Date = t.Date,
                NewBalance = t.NewBalance


            }).ToList();
            CustomerReference = customerId;
            AccountId = accountId;
        }

        public IActionResult OnGetFetchMore (int accountId, int pageNo)
        {
            var query = context.Accounts.Where(e => e.Id == accountId)
                    .SelectMany(e => e.Transactions)
                    .OrderByDescending(e => e.Date)
                ;
            var transaction = query.GetPaged(pageNo, 5);

            var list = transaction.Results.Select(t => new 
            {
                Id = accountId,
                Type = t.Type,
                Operation = t.Operation,
                Amount = t.Amount,
                Date = t.Date.ToString("g"),
                NewBalance = t.NewBalance


            }).ToList();
            //CustomerReference = customerId;
           

            return new JsonResult(new { items = list });
        }




        public IActionResult OnPostCustomer(int customerId)
        {
            Customer = context.Customers.Include(a => a.Accounts).First(c => c.Id == customerId);

            return RedirectToPage("CustomerPages/Customer", new {customerId});
        }
    }
}
