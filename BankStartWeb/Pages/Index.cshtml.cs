using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankStartWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;
        public List<Customer> Customers { get; set; }
        public List<Account> Accounts { get; set; }
        public List<Transaction> Transactions { get; set; }

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public decimal GetSum()
        {
            decimal sum = 0;
            foreach(var account in Accounts)
            {
                sum += account.Balance;
            }
            return Math.Round(sum);
                
                
        }
        public void OnGet()
        {
            Customers = _context.Customers.Select(c => new Customer()).ToList();
            Transactions = _context.Transactions.Select(t => new Transaction()).ToList();
            Accounts = _context.Accounts.Select(a => new Account
            {
                Balance = a.Balance,
            }).ToList();
            
                
        }
    }
}