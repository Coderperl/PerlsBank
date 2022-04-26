using System.ComponentModel.DataAnnotations;
using BankStartWeb.Data;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankStartWeb.Pages.CustomerPages
{[BindProperties]
    public class NewCustomerModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public NewCustomerModel(ApplicationDbContext context)
        {
            _context = context;

        }

        [MaxLength(50)] public string Givenname { get; set; }
        [MaxLength(50)] public string Surname { get; set; }
        [MaxLength(50)] public string Streetaddress { get; set; }
        [MaxLength(50)] public string City { get; set; }
        [MaxLength(10)] public string Zipcode { get; set; }
        [MaxLength(30)] public string Country { get; set; }
        [MaxLength(2)] public string CountryCode { get; set; }

        [MinLength(8, ErrorMessage = "Please enter 8 digits")] public string NationalId { get; set; }
        [Range(0, 9999)]
        public int TelephoneCountryCode { get; set; }
        public string Telephone { get; set; }
        [MaxLength(50)]
        public string EmailAddress { get; set; }
        public DateTime Birthday { get; set; }
        private static Random random = new Random();
        public List<SelectListItem> AllCountries { get; set; }
        


        
        public void OnGet()
        {
            SetAllCountries();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                var customer = new Customer();
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
                            customer.NationalId = NationalId + "-1111";
                            customer.TelephoneCountryCode = 46;
                            break;
                        case "Norge":
                            customer.CountryCode = "NO";
                            customer.NationalId = NationalId + "-2222";
                            customer.TelephoneCountryCode = 47;
                            break;
                        case "Finland":
                            customer.CountryCode = "FI";
                            customer.NationalId = NationalId + "-3333";
                            customer.TelephoneCountryCode = 48;
                            break;
                    }
                    customer.EmailAddress = EmailAddress;
                    customer.Telephone = Telephone;
                    customer.Birthday = Birthday;
                    //for (int i = 0; i < random.Next(1, 5); i++)
                    //{
                    //    customer.Accounts.Add(GenerateAccount());
                    //}
                    customer.Accounts = new List<Account>();
                    customer.Accounts.Add(new Account
                    {
                        AccountType = "Personal",
                        Balance = 0,
                        Created = DateTime.Now,
                        Transactions = new List<Transaction>()
                    });
                    _context.Customers.Add(customer);
                    _context.SaveChanges();
                }
                return RedirectToPage("/CustomerPages/Customers");
            }
            SetAllCountries();
            return Page();
        }

        public Account GenerateAccount()
        {
            string[] accountType = { "Personal", "Checking", "Savings" };
            var testUser = new Faker<Account>()
                .StrictMode(false)
                .RuleFor(e => e.Id, f => 0)
                .RuleFor(e => e.AccountType, (f, u) => f.PickRandom(accountType))
                .RuleFor(e => e.Balance, (f, u) => 0);

            var account = testUser.Generate(1).First();
            var start = DateTime.Now.AddDays(-random.Next(1000, 10000));
            account.Created = start;
            account.Balance = 0;
            var transactions = random.Next(1, 30);
            for (int i = 0; i < transactions; i++)
            {
                var tran = new Transaction();
                tran.Amount = random.NextInt64(1, 50) * 100;
                start = start.AddDays(random.NextInt64(50, 600));
                if (start > DateTime.Now)
                    break;
                tran.Date = start;
                account.Transactions.Add(tran);
                if (account.Balance - tran.Amount < 0)
                    tran.Type = "Debit";
                else
                {
                    if (random.NextInt64(0, 100) > 70)
                        tran.Type = "Debit";
                    else
                        tran.Type = "Credit";
                }

                var r = random.Next(0, 100);
                if (tran.Type == "Debit")
                {
                    account.Balance = account.Balance + tran.Amount;
                    if (r < 20)
                        tran.Operation = "Deposit cash";
                    else if (r < 66)
                        tran.Operation = "Salary";
                    else
                        tran.Operation = "Transfer";
                }
                else
                {
                    account.Balance = account.Balance - tran.Amount;
                    if (r < 40)
                        tran.Operation = "ATM withdrawal";
                    else if (r < 66)
                        tran.Operation = "Payment";
                    else
                        tran.Operation = "Transfer";
                }

                tran.NewBalance = account.Balance;
            }
            return account;
        }

        //public void SetAllTelephoneCountryCodes()
        //{
        //    AllTelephoneCountryCodes = new List<SelectListItem>()
        //    {
        //        new SelectListItem()
        //        {
        //            Text = "+46",
        //            Value = "46"
        //        },
        //        new SelectListItem()
        //        {
        //            Text = "+47",
        //            Value = "47"
        //        },
        //        new SelectListItem()
        //        {
        //            Text = "+48",
        //            Value = "48"
        //        }
        //    };
        //}

        //public void SetAllCountryCodes()
        //{
        //    AllCountryCodes = new List<SelectListItem>()
        //    {
        //        new SelectListItem()
        //        {
        //            Text = "SE",
        //            Value = "SE"
        //        },
        //        new SelectListItem()
        //        {
        //            Text = "NO",
        //            Value = "NO"
        //        },
        //        new SelectListItem()
        //        {
        //            Text = "FI",
        //            Value = "FI"
        //        }
        //    };
        //}

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
