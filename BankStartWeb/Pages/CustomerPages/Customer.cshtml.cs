using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankStartWeb.Pages
{
    public class CustomerModel : PageModel
    {
        private readonly ApplicationDbContext context;

        public string Givenname { get; set; }
        public string Surname { get; set; }
        public string Streetaddress { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string NationalId { get; set; }
        public int TelephoneCountryCode { get; set; }
        public string Telephone { get; set; }
        public string EmailAddress { get; set; }
        public DateTime Birthday { get; set; }

        public List<AccountViewModel> Accounts { get; set; } 
        
        public Customer Customer { get; set; }

        public class AccountViewModel
        {
            public int Id { get; set; }
            public string AccountType { get; set; }
            public DateTime Created { get; set; }
            public decimal Balance { get; set; }
            public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        }

        public CustomerModel(ApplicationDbContext _context)
        {
            context = _context;
        }
        

        public void OnGet(int customerId)
        {
            
            Customer = context.Customers.Include(a => a.Accounts). First(c => c.Id == customerId);
            Accounts = Customer.Accounts.Select(a => new AccountViewModel
            {
                Id = a.Id,
                AccountType = a.AccountType,
                Balance = a.Balance,

            }).ToList();

            
        }
    }
}
