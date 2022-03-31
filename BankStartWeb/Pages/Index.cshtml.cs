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

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            Customers = _context.Customers.Select(Accounts => new Customer()).ToList();
            Accounts = _context.Accounts.Include(t => t.Transactions).Select(a => new Account
            {
                Balance = a.Balance,
            }).ToList();
            
                
        }
    }
}