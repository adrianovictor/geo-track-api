using System.Linq.Expressions;
using Core.Common.Domain.Entities;
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

    public async Task DeleteAsync<T>(T entity) where T : Entity<int>
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public IQueryable<T> GetAllAsync<T>() where T : Entity<int>
    {
        return _context.Set<T>();
    }

    public IQueryable<T> GetQueryable<T>() where T : Entity<int>
    {
        return _context.Set<T>();//.AsNoTracking();
    }

    public async Task<T?> GetFirstOrDefaultAsync<T>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>>[] includeProperties = null,
        CancellationToken cancellationToken = default
    ) where T : Entity<int>
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

    public async Task SaveAsync<T>(T entity, bool disableValidationOnSave) where T : Entity<int>
    {
        if (entity.IsTransient())
        // {
        //     _context.Entry(entity).State = EntityState.Modified;
        // }
        // else
        {
            _context.Set<T>().Add(entity);
        }

        await _context.SaveChangesAsync();
    }
}
