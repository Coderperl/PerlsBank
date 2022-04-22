using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var status = _sut.Deposit(1, -1);
            Assert.AreEqual(ITransactionServices.Status.LowerThanZero,status);
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
            var status = _sut.Withdrawal(2, -1);
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
                Id = 3,
                Transactions = new()
            };
            _context.Accounts.Add(a);
            _context.SaveChanges();
            var status = _sut.Withdrawal(3, 150);
            Assert.AreEqual(ITransactionServices.Status.InsufficientFunds, status);
        }


    }
}
