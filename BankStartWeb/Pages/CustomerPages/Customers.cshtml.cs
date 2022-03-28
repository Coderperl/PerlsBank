using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankStartWeb.Pages
{
    public class CustomersModel : PageModel
    {
        public class CustomersViewModel
        {
            public int Id { get; set; }
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

            public List<Account> Accounts { get; set; } = new List<Account>();

        }
        private readonly ApplicationDbContext context;
        public List<CustomersViewModel> Customers = new List<CustomersViewModel>();
        
        public CustomersModel(ApplicationDbContext _context)
        {
            context = _context;
        }
        public void OnGet()
        {
            Customers = context.Customers.Take(30).Select(c => new CustomersViewModel
            {
                Id = c.Id,
                Givenname = c.Givenname,
                Surname = c.Surname,
                EmailAddress = c.EmailAddress,
                Streetaddress = c.Streetaddress,
                City = c.City,
                
            }).ToList();
        }
        
    }
}
