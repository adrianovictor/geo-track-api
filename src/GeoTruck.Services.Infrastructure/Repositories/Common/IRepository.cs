using System.Linq.Expressions;
using GeoTruck.Services.Domain.Common;

namespace GeoTruck.Services.Infrastructure.Repositories.Common;

public interface IRepository : IDisposable
{
    IQueryable<T> GetAllAsync<T>() where T : class, IEntity<T>;
    IQueryable<T> GetQueryable<T>() where T : class, IEntity<T>;
    Task<T?> GetFirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken) where T : class, IEntity<T>;
    Task SaveAsync<T>(T entity, bool disableValidationOnSave = false) where T : class, IEntity<T>;
    Task DeleteAsync<T>(T entity) where T : class, IEntity<T>;
}
