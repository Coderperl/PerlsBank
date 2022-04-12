using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankStartWeb.Pages
{
    public class CustomerModel : PageModel
    {
        private readonly ApplicationDbContext context;

        public List<AccountViewModel> Accounts { get; set; }

        public Customer Customer { get; set; }

        public class AccountViewModel
        {
            public int CustomerId { get; set; }
            public int Id { get; set; }
            public string AccountType { get; set; }
            public DateTime Created { get; set; }
            public decimal Balance { get; set; }

        }

        public CustomerModel(ApplicationDbContext _context)
        {
            context = _context;
        }


        public IActionResult OnGet(int customerId)
        {
            Customer = context.Customers.Find(customerId);
            //Customer = context.Customers.Include(a => a.Accounts).FirstOrDefault(c => c.Id == customerId);

            if (Customer == default) return RedirectToPage("/CustomerPages/Customers");
            context.Entry(Customer).Collection(c => c.Accounts).Load();

            Accounts = Customer.Accounts.Select(a => new AccountViewModel
            {
                Id = a.Id,
                AccountType = a.AccountType,
                Balance = a.Balance,
                Created = a.Created,
                CustomerId = customerId,

            }).ToList();
            return Page();
        }
    }
}
