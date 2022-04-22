using System.ComponentModel.DataAnnotations;
using BankStartWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;

namespace BankStartWeb.Services
{
    public class TransactionActions : ITransactionServices
    {
        private readonly ApplicationDbContext _context;

        

        public TransactionActions(ApplicationDbContext context)
        {
            _context = context;
        }
        public string Operation { get; set; }
        public string Type { get; set; }
        [Range(100, Int16.MaxValue, ErrorMessage = "Please deposit an amount not less than 100")]
        public decimal Amount { get; set; }
        public ITransactionServices.Status Deposit(int accountId, decimal amount)
        {
            if (amount < 0)
            {
                return ITransactionServices.Status.LowerThanZero;
            }
            
            var account = _context.Accounts.Include(t => t.Transactions).First(a => a.Id == accountId);
            account.Transactions.Add(new Transaction
            {
                Amount = amount,
                Operation = "Deposit cash",
                Date = DateTime.Now,
                Type = "Debit",
                NewBalance = account.Balance + Amount
            });

            account.Balance += Amount;
            return ITransactionServices.Status.Ok;

        }

        public ITransactionServices.Status Withdrawal(int accountId, decimal amount)
        {
            if (amount < 0)
            {
                return ITransactionServices.Status.LowerThanZero;
            }
            var account = _context.Accounts.Include(t => t.Transactions).First(a => a.Id == accountId);
            account.Transactions.Add(new Transaction
            {
                Amount = amount,
                Operation = "ATM withdrawal",
                Date = DateTime.Now,
                Type = "Credit",
                NewBalance = account.Balance - amount
            });
            account.Balance -= amount;
            if (amount > account.Balance)
            {
                return ITransactionServices.Status.InsufficientFunds;
            }
            return ITransactionServices.Status.Ok;
        }

        public ITransactionServices.Status Transfer(int accountId, decimal amount)
        {
            throw new NotImplementedException();
        }
    }
}
