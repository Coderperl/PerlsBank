using System;
using System.Collections.Generic;
using System.Linq;
using BankStartWeb.Data;
using BankStartWeb.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BankTests.Services
{
    [TestClass]
    public class TransactionTests
    {
        private readonly ApplicationDbContext _context;
        private readonly TransactionActions _sut;

        public TransactionTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;
            _context = new ApplicationDbContext(options);
            _sut = new TransactionActions(_context);
        }

        [TestMethod]
        public void If_Deposit_Negative_Amount_Should_Return_Below_Zero()
        {
            var a = new Account()
            {
                AccountType = "test",
                Balance = 1,
                Created = DateTime.Now,
                Id = 1,
                Transactions = new()
            };
            _context.Accounts.Add(a);
            _context.SaveChanges();
            var status = _sut.Deposit(a.Id, -1);
            Assert.AreEqual(ITransactionServices.Status.LowerThanZero,status);
        }
        [TestMethod]
        public void If_Deposit_Operation_Returns_Correct_Operation()
        {
            var a = new Account()
            {
                AccountType = "test",
                Balance = 1000,
                Created = DateTime.Now,
                Id = 3,
                Transactions = new()
            };
            _context.Accounts.Add(a);
            _context.SaveChanges();
            _sut.Deposit(a.Id, 100);
            var transaction = a.Transactions.Last();
            Assert.AreEqual("Deposit cash", transaction.Operation);
        }
        [TestMethod]
        public void If_Withdrawal_Negative_Amount_Should_Return_Below_Zero()
        {
            var a = new Account()
            {
                AccountType = "test",
                Balance = 1,
                Created = DateTime.Now,
                Id = 2,
                Transactions = new()
            };
            _context.Accounts.Add(a);
            _context.SaveChanges();
            var status = _sut.Withdrawal(a.Id, -1);
            Assert.AreEqual(ITransactionServices.Status.LowerThanZero, status);
        }
        [TestMethod]
        public void If_Withdrawal_Amount_To_Big_Should_Return_InsufficientFunds()
        {
            var a = new Account()
            {
                AccountType = "test",
                Balance = 0,
                Created = DateTime.Now,
                Id = 4,
                Transactions = new()
            };
            _context.Accounts.Add(a);
            _context.SaveChanges();
            var status = _sut.Withdrawal(a.Id, 150);
            Assert.AreEqual(ITransactionServices.Status.InsufficientFunds, status);
        }
        [TestMethod]
        public void If_Withdrawal_Operation_Returns_Correct_Operation()
        {
            var a = new Account()
            {
                AccountType = "test",
                Balance = 1000,
                Created = DateTime.Now,
                Id = 5,
                Transactions = new()
            };
            _context.Accounts.Add(a);
            _context.SaveChanges();
            _sut.Withdrawal(a.Id, 100);
            var transaction = a.Transactions.Last();
            Assert.AreEqual("ATM withdrawal", transaction.Operation );
        }
        [TestMethod]
        public void If_Transfer_Negative_Amount_Should_Return_Below_Zero()
        {
            var sender = new Account()
            {
                AccountType = "test",
                Balance = 1000,
                Created = DateTime.Now,
                Id = 6,
                Transactions = new()
            };
            var receiver = new Account()
            {
                AccountType = "test",
                Balance = 2000,
                Created = DateTime.Now,
                Id = 7,
                Transactions = new()
            };
            _context.Accounts.Add(sender);
            _context.Accounts.Add(receiver);
            _context.SaveChanges();
            var status = _sut.Transfer(sender.Id,receiver.Id, -1);
            Assert.AreEqual(ITransactionServices.Status.LowerThanZero, status);
        }
        [TestMethod]
        public void If_Transfer_Amount_Should_Return_InsufficientFunds()
        {
            var sender = new Account()
            {
                AccountType = "test",
                Balance = 1000,
                Created = DateTime.Now,
                Id = 8,
                Transactions = new()
            };
            var receiver = new Account()
            {
                AccountType = "test",
                Balance = 2000,
                Created = DateTime.Now,
                Id = 9,
                Transactions = new()
            };
            _context.Accounts.Add(sender);
            _context.Accounts.Add(receiver);
            _context.SaveChanges();
            var status = _sut.Transfer(sender.Id, receiver.Id, 2000);
            Assert.AreEqual(ITransactionServices.Status.InsufficientFunds, status);
        }

        [TestMethod]
        public void If_Transfer_Operation_Returns_Correct_Operation()
        {
            var sender = new Account()
            {
                AccountType = "test",
                Balance = 1000,
                Created = DateTime.Now,
                Transactions = new()
            };
            var receiver = new Account()
            {
                AccountType = "test",
                Balance = 2000,
                Created = DateTime.Now,
                Transactions = new()
            };
            _context.Accounts.Add(sender);
            _context.Accounts.Add(receiver);
            _context.SaveChanges();
            _sut.Transfer(sender.Id, receiver.Id, 100);
            var senderTransaction = sender.Transactions.Last();
            var receiverTransaction = receiver.Transactions.Last();
            

            Assert.AreEqual("Transfer", senderTransaction.Operation);
            Assert.AreEqual("Transfer", receiverTransaction.Operation);
        }
        [TestMethod]
        public void If_Transfer_Type_Returns_Correct_Transaction_Type()
        {
            var sender = new Account()
            {
                AccountType = "test",
                Balance = 1000,
                Created = DateTime.Now,
                Transactions = new()
            };
            var receiver = new Account()
            {
                AccountType = "test",
                Balance = 2000,
                Created = DateTime.Now,
                Transactions = new()
            };
            _context.Accounts.Add(sender);
            _context.Accounts.Add(receiver);
            _context.SaveChanges();
            _sut.Transfer(sender.Id, receiver.Id, 100);
            var senderTransaction = sender.Transactions.Last();
            var receiverTransaction = receiver.Transactions.Last();


            Assert.AreEqual("Credit", senderTransaction.Type);
            Assert.AreEqual("Debit", receiverTransaction.Type);
        }
    }
}
