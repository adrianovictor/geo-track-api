using System.Linq.Expressions;
using Core.Common.Domain.Entities;
using GeoTruck.Services.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;

namespace GeoTruck.Services.Infrastructure.Repositories.Common;

public class EntityRepository(ApplicationDbContext context) : IRepository, IDisposable
{
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
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync();
    }

    public IQueryable<T> GetAllAsync<T>() where T : Entity<int>
    {
        return context.Set<T>();
    }

    public IQueryable<T> GetQueryable<T>() where T : Entity<int>
    {
        return context.Set<T>();//.AsNoTracking();
    }

    public async Task<T?> GetFirstOrDefaultAsync<T>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>>[] includeProperties = null,
        CancellationToken cancellationToken = default
    ) where T : Entity<int>
    {
        IQueryable<T> query = context.Set<T>();

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
        //     context.Entry(entity).State = EntityState.Modified;
        // }
        // else
        {
            context.Set<T>().Add(entity);
        }

        await context.SaveChangesAsync();
    }
}
