using Contracts.Models;
using DistributedBanking.Processing.ReplicaSet.Domain.Models.Transaction;

namespace DistributedBanking.Processing.ReplicaSet.Domain.Services;

public interface ITransactionService
{
    Task<OperationResult> Deposit(OneWayTransactionModel depositTransactionModel);
    Task<OperationResult> Withdraw(OneWaySecuredTransactionModel withdrawTransactionModel);
    Task<OperationResult> Transfer(TwoWayTransactionModel transferTransactionModel);
    Task<decimal> GetBalance(string accountId);
    Task<IEnumerable<TransactionResponseModel>> GetAccountTransactionHistory(string accountId);
}