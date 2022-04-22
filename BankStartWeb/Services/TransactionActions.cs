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
                NewBalance = account.Balance + amount
            });
            account.Balance += amount;
            if (_context.SaveChanges() > 0)
            {
                return ITransactionServices.Status.Ok;
            }
            return ITransactionServices.Status.Error;
        }

        public ITransactionServices.Status Withdrawal(int accountId, decimal amount)
        {
            switch (amount)
            {
                case < 0:
                    return ITransactionServices.Status.LowerThanZero;
                case < 100:
                    return ITransactionServices.Status.MinimumWithdrawal;
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
            _context.SaveChanges();
            return amount > account.Balance ? ITransactionServices.Status.InsufficientFunds : ITransactionServices.Status.Ok;
        }

        public ITransactionServices.Status Transfer(int thisAccountId, int receiverAccountId, decimal amount)
        {
            if (amount < 0)
            {
                return ITransactionServices.Status.LowerThanZero;
            }
            var senderAccount = _context.Accounts.Include(t => t.Transactions).First(a => a.Id == thisAccountId);
            var receiverAccount = _context.Accounts.Include(t => t.Transactions).First(a => a.Id == receiverAccountId);
            var sender = new Transaction
            {
                Amount = amount,
                Operation = "Credit",
                Date = DateTime.Now,
                Type = "Transfer",
                NewBalance = senderAccount.Balance - amount
            };
            if (amount > senderAccount.Balance)
            {
                return ITransactionServices.Status.InsufficientFunds;
            }
            var reciever = new Transaction
            {
                Amount = amount,
                Operation = "Debit",
                Date = DateTime.Now,
                Type = "Transfer",
                NewBalance = receiverAccount.Balance + amount
            };

            senderAccount.Balance -= amount;
            receiverAccount.Balance += amount;
            senderAccount.Transactions.Add(sender);
            receiverAccount.Transactions.Add(reciever);
            _context.SaveChanges();
            return ITransactionServices.Status.Ok;
        }
    }
    
}
