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
        public string Searchterm { get; set; }
        public string SSN { get; set; }

        public CustomersModel(ApplicationDbContext _context)
        {
            context = _context;
        }
        public void OnGet(string searchterm, string col="id", string order ="asc")
        {
            var cust = context.Customers.AsQueryable();
            Searchterm = searchterm;
            if (!string.IsNullOrEmpty(Searchterm))
                cust = cust.Where(word => word.Givenname.Contains(Searchterm)
                || word.Surname.Contains(Searchterm)
                || word.City.Contains(Searchterm));

            else if (col == "id")
            {
                if (order == "asc")
                {
                    cust = cust.OrderBy(word => word.Id);
                }
                else
                {
                    cust = cust.OrderByDescending(word => word.Id);
                }

            }
            else if (col == "NationalId")
            {
                if (order == "asc")
                {
                    cust = cust.OrderBy(word => word.NationalId);
                }
                else
                {
                    cust = cust.OrderByDescending(word => word.NationalId);
                }

            }
            else if (col == "firstname")
            {
                if (order == "asc")
                {
                    cust = cust.OrderBy(word => word.Givenname);
                }
                else
                {
                    cust = cust.OrderByDescending(word => word.Givenname);
                }

            }
            else if (col == "lastname")
            {
                if (order == "asc")
                {
                    cust = cust.OrderBy(word => word.Surname);
                }
                else
                {
                    cust = cust.OrderByDescending(word => word.Surname);
                }

            }
            else if (col == "city")
            {
                if (order == "asc")
                {
                    cust = cust.OrderBy(word => word.City);
                }
                else
                {
                    cust = cust.OrderByDescending(word => word.City);
                }

            }
            else if (col == "address")
            {
                if (order == "asc")
                {
                    cust = cust.OrderBy(word => word.Streetaddress);
                }
                else
                {
                    cust = cust.OrderByDescending(word => word.Streetaddress);
                }

            }

            Customers = cust.Take(30).Select(c => new CustomersViewModel
            {
                Id = c.Id,
                NationalId = c.NationalId,
                Givenname = c.Givenname,
                Surname = c.Surname,
                EmailAddress = c.EmailAddress,
                Streetaddress = c.Streetaddress,
                City = c.City,
                
            }).ToList();
        }

        //public IActionResult OnPostCustomerPage(string SSN)
        //{
           

        //}
        
    }
}
