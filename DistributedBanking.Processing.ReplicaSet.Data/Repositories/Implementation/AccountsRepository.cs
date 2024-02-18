using DistributedBanking.Processing.ReplicaSet.Data.Repositories.Base;
using Shared.Data.Entities;
using Shared.Data.Entities.Constants;
using Shared.Data.Services;

namespace DistributedBanking.Processing.ReplicaSet.Data.Repositories.Implementation;

public class AccountsRepository : RepositoryBase<AccountEntity>, IAccountsRepository
{
    public AccountsRepository(IMongoDbFactory mongoDbFactory) 
        : base(
            mongoDbFactory.GetDatabase(), 
            CollectionNames.Accounts)
    {
        
    }
}