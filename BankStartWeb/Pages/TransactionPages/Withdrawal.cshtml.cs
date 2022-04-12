using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BankStartWeb.Pages.Transactions;

[BindProperties]
public class WithdrawalModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public WithdrawalModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public int AccountId { get; set; }
    public int CustomerId { get; set; }
    public string Operation { get; set; }
    public string Type { get; set; }
    public decimal Amount { get; set; }
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
            var withdrawal = new Transaction
            {
                Amount = Amount,
                Operation = Operation,
                Date = DateTime.Now,
                Type = Type,
                NewBalance = Account.Balance - Amount
            };
            if (withdrawal.NewBalance < 0)
            {
                ModelState.AddModelError(nameof(Amount),
                    "Insufficient funds");
                SetSelectLists();
                return Page();
            }
            if (Amount < 100)
            {
                ModelState.AddModelError(nameof(Amount),
                    "Please enter an amount of 100 or more");
                SetSelectLists();
                return Page();
            }

            Account.Transactions.Add(withdrawal);
            Account.Balance -= Amount;
            _context.SaveChanges();
            return RedirectToPage("/CustomerPages/Customer", new { customerId });
        }
        SetSelectLists();
        return Page();

    }

    public void SetSelectLists()
    {
        SetTypes();
        SetOperations();
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
                Text = "Payment",
                Value = "Payment"
            },
            new()
            {
                Text = "ATM withdrawal",
                Value = "ATM withdrawal"
            }
        };
    }
}