using System.ComponentModel.DataAnnotations;
using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BankStartWeb.Pages.Transactions
{
    [BindProperties]
    public class DepositModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DepositModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string Operation { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public Customer Customer { get; set; }
        public List<SelectListItem> AllTypes { get; set; }
        public List<SelectListItem> AllOperations { get; set; }

        public void OnGet(int accountId, int customerId)
        {
            AccountId = accountId;
            CustomerId = customerId;
            SetSelectLists();
        }

        public IActionResult OnPost(int accountId, int customerId)
        {
            if (ModelState.IsValid)
            {
                Customer = _context.Customers.First(c => c.Id == customerId);
                Account = _context.Accounts.Include(t => t.Transactions).First(a => a.Id == accountId);
                Account.Transactions.Add(new Transaction
                {
                    Amount = Amount,
                    Operation = Operation,
                    Date = DateTime.Now,
                    Type = Type,
                    NewBalance = Account.Balance + Amount
                });
                Account.Balance = Account.Balance + Amount;
                _context.SaveChanges();
            }

            return RedirectToPage("/CustomerPages/Customer", new {customerId});
        }
        public void SetSelectLists()
        {
            SetTypes();
            SetOperations();
        }
        private void SetTypes()
        {
            AllTypes = new List<SelectListItem>()
            {
                new SelectListItem {Text = "Debit", Value = "Debit"},
                new SelectListItem {Text = "Credit", Value = "Credit"}
            };
        }

        private void SetOperations()
        {
            AllOperations = new List<SelectListItem>()
            {
                new SelectListItem {Text = "Deposit Cash", Value = "Deposit cash"},
                new SelectListItem {Text = "Salary", Value = "Salary"},
            };
        }
    }
}