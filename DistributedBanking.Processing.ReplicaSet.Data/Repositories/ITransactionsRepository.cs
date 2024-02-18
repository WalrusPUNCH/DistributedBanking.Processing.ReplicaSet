using DistributedBanking.Processing.ReplicaSet.Data.Repositories.Base;
using Shared.Data.Entities;

namespace DistributedBanking.Processing.ReplicaSet.Data.Repositories;

public interface ITransactionsRepository : IRepositoryBase<TransactionEntity>
{
    Task<IEnumerable<TransactionEntity>> AccountTransactionHistory(string accountId);
}