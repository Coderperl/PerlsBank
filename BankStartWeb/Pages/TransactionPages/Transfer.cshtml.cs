using System.ComponentModel.DataAnnotations;
using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BankStartWeb.Pages.TransactionPages
{[BindProperties]
    public class TransferModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TransferModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public int AccountId { get; set; }
        [Range(1,Int16.MaxValue, ErrorMessage = "Invalid account number")]
        public int TransferId { get; set; }
        public int CustomerId { get; set; }
        public string Operation { get; set; }
        public string Type { get; set; }
        [Range(100,25000, ErrorMessage = "Enter an amount between 100 and 25000")]
        public decimal Amount { get; set; }
        public Account Account { get; set; }
        public Customer Customer { get; set; }
        public List<Account> Accounts { get; set; }
        public List<SelectListItem> AllTypes { get; set; }
        public List<SelectListItem> AllOperations { get; set; }
        public List<SelectListItem> ReceivingAccounts { get; set; }
        public void OnGet(int accountId, int customerId)
        {
            Customer = _context.Customers.Include(a => a.Accounts).First(c => c.Id == customerId);
            Accounts = Customer.Accounts.Select(a => new Account 
            {
                Id = a.Id,

            }).ToList();
            AccountId = accountId;
            CustomerId = customerId;
            SetLists();
        }

        public IActionResult OnPost(int customerId, int accountId)
        {

            if (ModelState.IsValid)
            {
                Customer = _context.Customers.First(c => c.Id == customerId);
                var senderAccount = _context.Accounts.Include(t => t.Transactions).First(a => a.Id == accountId);
                var receiverAccount = _context.Accounts.Include(t => t.Transactions).First(a => a.Id == TransferId);
                var sender = new Transaction
                {
                    Amount = Amount,
                    Operation = Operation,
                    Date = DateTime.Now,
                    Type = Type,
                    NewBalance = senderAccount.Balance - Amount
                };
                var reciever = new Transaction
                {
                    Amount = Amount,
                    Operation = Operation,
                    Date = DateTime.Now,
                    Type = Type,
                    NewBalance = receiverAccount.Balance + Amount
                };
                if (sender.NewBalance < 0)
                {
                    ModelState.AddModelError(nameof(Amount),
                        "Insufficient funds");
                }
                else {
                senderAccount.Balance -= Amount;
                receiverAccount.Balance += Amount;
                senderAccount.Transactions.Add(sender);
                receiverAccount.Transactions.Add(reciever);

                _context.SaveChanges();
                return RedirectToPage("/CustomerPages/Customer", new { customerId });
                }
            }

            SetLists();
            return Page();

        }
        private void SetLists()
        {
            SetReceivingAccount();
            SetTypes();
            SetOperations();
        }
        private void SetReceivingAccount()
        {
            ReceivingAccounts = Customer.Accounts.Select(a => new SelectListItem
            {
                Text = a.Id.ToString(),
                Value = a.Id.ToString()

            }).ToList();
        }
        private void SetTypes()
        {
            AllTypes = new List<SelectListItem>
            {
                new()
                {
                    Text = "Debit",
                    Value = "Debit"
                },
                new()
                {
                    Text = "Credit",
                    Value = "Credit"
                }
            };
        }
        private void SetOperations()
        {
            AllOperations = new List<SelectListItem>
            {
                new()
                {
                    Text = "Transfer",
                    Value = "Transfer"
                },
               
            };
        }
    }
}
