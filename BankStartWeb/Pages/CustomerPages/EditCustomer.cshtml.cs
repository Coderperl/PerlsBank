using System.ComponentModel.DataAnnotations;
using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BankStartWeb.Pages.CustomerPages
{[BindProperties]
    public class EditCustomerModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditCustomerModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [MaxLength(50)] public string Givenname { get; set; }
        [MaxLength(50)] public string Surname { get; set; }
        [MaxLength(50)] public string Streetaddress { get; set; }
        [MaxLength(50)] public string City { get; set; }
        [MaxLength(10)] public string Zipcode { get; set; }
        [MaxLength(30)] public string Country { get; set; }
        [MinLength(8)] public string NationalId { get; set; }
        public string Telephone { get; set; }
        [MaxLength(50)] public string EmailAddress { get; set; }
        public DateTime Birthday { get; set; }
        private static Random random = new Random();
        public List<SelectListItem> AllCountries { get; set; }
        

        public void OnGet(int customerId)
        {
            
            var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId);
            {
                Givenname = customer.Givenname;
                Surname = customer.Surname  ;
                Streetaddress = customer.Streetaddress ; 
                City = customer.City ;
                Zipcode = customer.Zipcode; 
                Country = customer.Country;
                EmailAddress = customer.EmailAddress;
                Telephone = customer.Telephone;
                NationalId = customer.NationalId;
                Birthday= customer.Birthday ;
                SetAllCountries();
            }
        }

        public IActionResult OnPost(int customerId)
        {
            if (ModelState.IsValid)
            {
                var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId);
                {
                    customer.Givenname = Givenname;
                    customer.Surname = Surname;
                    customer.Streetaddress = Streetaddress;
                    customer.City = City;
                    customer.Zipcode = Zipcode;
                    customer.Country = Country;
                    switch (Country)
                    {
                        case "Sverige":
                            customer.CountryCode = "SE";
                            customer.TelephoneCountryCode = 46;
                            customer.NationalId = NationalId + "-1111";
                            break;
                        case "Norge":
                            customer.CountryCode = "NO";
                            customer.TelephoneCountryCode = 47;
                            customer.NationalId = NationalId + "-2222";
                            break;
                        case "Finland":
                            customer.CountryCode = "FI";
                            customer.TelephoneCountryCode = 48;
                            customer.NationalId = NationalId + "-3333";
                            break;
                    }
                    customer.EmailAddress = EmailAddress;
                    customer.Telephone = Telephone;
                    customer.Birthday = Birthday;
                    _context.SaveChanges();
                }
                return RedirectToPage("/CustomerPages/Customer");
            }
            SetAllCountries();
            return Page();
        }
        public void SetAllCountries()
        {
            AllCountries = new List<SelectListItem>
            {
                new SelectListItem()
                {
                    Text = "Sverige",
                    Value = "Sverige"
                },
                new SelectListItem()
                {
                    Text = "Norge",
                    Value = "Norge"
                },
                new SelectListItem()
                {
                    Text = "Finland",
                    Value = "Finland"
                }
            };
        }
    }
}
