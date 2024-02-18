using DistributedBanking.Processing.ReplicaSet.Data.Repositories.Base;
using Shared.Data.Entities.Identity;

namespace DistributedBanking.Processing.ReplicaSet.Data.Repositories;

public interface IUsersRepository : IRepositoryBase<ApplicationUser>
{
    Task<ApplicationUser?> GetByEmailAsync(string email);
}