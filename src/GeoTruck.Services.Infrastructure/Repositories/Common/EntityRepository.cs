using System.Linq.Expressions;
using GeoTruck.Services.Domain.Common;
using GeoTruck.Services.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;

namespace GeoTruck.Services.Infrastructure.Repositories.Common;

public class EntityRepository(ApplicationDbContext context) : IRepository, IDisposable
{
    private readonly ApplicationDbContext _context = context;
    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
            }

            _disposed = true;
        }
    }

    public async Task DeleteAsync<T>(T entity) where T : class, IEntity<T>
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public IQueryable<T> GetAllAsync<T>() where T : class, IEntity<T>
    {
        return _context.Set<T>();
    }

    public IQueryable<T> GetQueryable<T>() where T : class, IEntity<T>
    {
        return _context.Set<T>().AsNoTracking();
    }

    public async Task<T?> GetFirstOrDefaultAsync<T>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>>[] includeProperties = null,
        CancellationToken cancellationToken = default
    ) where T : class, IEntity<T>
    {
        IQueryable<T> query = _context.Set<T>();

        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query
            .Where(predicate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SaveAsync<T>(T entity, bool disableValidationOnSave) where T : class, IEntity<T>
    {
        if (entity.IsPersisted())
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
        else
        {
            _context.Set<T>().Add(entity);
        }

        await _context.SaveChangesAsync();
    }
}
