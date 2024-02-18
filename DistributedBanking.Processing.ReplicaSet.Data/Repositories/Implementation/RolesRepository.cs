using DistributedBanking.Processing.ReplicaSet.Data.Repositories.Base;
using Shared.Data.Entities.Constants;
using Shared.Data.Entities.Identity;
using Shared.Data.Services;

namespace DistributedBanking.Processing.ReplicaSet.Data.Repositories.Implementation;

public class RolesRepository : RepositoryBase<ApplicationRole>, IRolesRepository
{
    public RolesRepository(IMongoDbFactory mongoDbFactory) 
        : base(
            mongoDbFactory.GetDatabase(), 
            CollectionNames.Service.Roles)
    {
        
    }
}