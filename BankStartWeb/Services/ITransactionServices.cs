namespace BankStartWeb.Services
{
    public interface ITransactionServices
    {
        public Status Deposit(int accountId, decimal amount);
        public Status Withdrawal(int accountId, decimal amount);
        public Status Transfer(int accountId, int receiverId, decimal amount);

        public enum Status
        {
            Ok,
            MinimumWithdrawal, 
            Error,
            LowerThanZero,
            InsufficientFunds
        }
    }
}
