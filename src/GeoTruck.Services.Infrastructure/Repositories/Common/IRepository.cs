using System.Linq.Expressions;
using Core.Common.Domain.Entities;

namespace GeoTruck.Services.Infrastructure.Repositories.Common;

public interface IRepository : IDisposable
{
    IQueryable<T> GetAllAsync<T>() where T : Entity<int>;
    IQueryable<T> GetQueryable<T>() where T : Entity<int>;
    Task<T?> GetFirstOrDefaultAsync<T>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>>[] includeProperties = null,
        CancellationToken cancellationToken = default) where T : Entity<int>;
    Task SaveAsync<T>(T entity, bool disableValidationOnSave = false) where T : Entity<int>;
    Task DeleteAsync<T>(T entity) where T : Entity<int>;
}
