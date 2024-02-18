using DistributedBanking.Processing.ReplicaSet.Data.Repositories.Base;
using Shared.Data.Entities.Constants;
using Shared.Data.Entities.EndUsers;
using Shared.Data.Services;

namespace DistributedBanking.Processing.ReplicaSet.Data.Repositories.Implementation;

public class CustomersRepository : RepositoryBase<CustomerEntity>, ICustomersRepository
{
    public CustomersRepository(IMongoDbFactory mongoDbFactory) 
        : base(
            mongoDbFactory.GetDatabase(), 
            CollectionNames.EndUsers)
    {
        
    }
}