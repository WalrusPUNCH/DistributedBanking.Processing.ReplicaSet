using Contracts.Extensions;
using DistributedBanking.Processing.ReplicaSet.Data.Repositories.Base;
using Shared.Data.Entities.Constants;
using Shared.Data.Entities.Identity;
using Shared.Data.Services;

namespace DistributedBanking.Processing.ReplicaSet.Data.Repositories.Implementation;

public class UsersRepository : RepositoryBase<ApplicationUser>, IUsersRepository
{
    public UsersRepository(IMongoDbFactory mongoDbFactory)
        : base(
            mongoDbFactory.GetDatabase(),
            CollectionNames.Service.Users)
    {
        
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return (await GetAsync(u => u.NormalizedEmail == email.NormalizeString())).FirstOrDefault();
    }
}