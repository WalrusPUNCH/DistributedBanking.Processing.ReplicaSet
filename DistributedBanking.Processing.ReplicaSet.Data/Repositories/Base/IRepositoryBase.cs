using System.Linq.Expressions;
using MongoDB.Bson;
using Shared.Data.Entities;

namespace DistributedBanking.Processing.ReplicaSet.Data.Repositories.Base;

public interface IRepositoryBase<T> where T : BaseEntity
{
    Task AddAsync(T entity);
    Task<IReadOnlyCollection<T>> GetAllAsync();
    Task<T?> GetAsync(ObjectId id);
    Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? filter = null);
    Task RemoveAsync(ObjectId id);
    Task UpdateAsync(T entity);
}