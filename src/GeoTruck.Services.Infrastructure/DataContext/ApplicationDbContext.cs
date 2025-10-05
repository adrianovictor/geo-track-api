using GeoTruck.Services.Domain.Common;
using GeoTruck.Services.Domain.Entities;
using GeoTruck.Services.Infrastructure.Mappings.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace GeoTruck.Services.Infrastructure.DataContext;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<VehicleLocation> VehicleLocations { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new VehicleMap());
        modelBuilder.ApplyConfiguration(new VehicleLocationMap());

    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Save(async () =>
        {
            return await base.SaveChangesAsync(cancellationToken).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // TODO: Adicionar log aqui
                }

                return task.Result;
            }, cancellationToken);
        });
    }

    protected virtual async Task<int> Save(Func<Task<int>> action)
    {
        int affectedRows;

        try
        {
            var entries = GetEntities();

            TraceAudit(entries);
            
            affectedRows = await action();

            await Task.CompletedTask;
        }
        catch (Exception)
        {
            // TODO: adicionar log aqui
            throw;
        }

        return affectedRows;
    }

    private IDictionary<object, EntityState> GetEntities()
    {
        return ChangeTracker.Entries()
            .Where(_ => _.State == EntityState.Added ||
                        _.State == EntityState.Modified ||
                        _.State == EntityState.Deleted ||
                        _.State == EntityState.Unchanged)
            .Select(_ => new { _.Entity, _.State })
            .DistinctBy(_ => _.Entity)
            .ToDictionary(_ => _.Entity, _ => _.State);
    }

    protected void TraceAudit(IDictionary<object, EntityState> entries)
    {
        var modifyEntries = entries.Where(_ => _.Value != EntityState.Unchanged);

        foreach (var entry in modifyEntries)
        {
            var auditEntity = entry.Key as IAuditing;

            if (entry.Value == EntityState.Added && auditEntity != null)
            {
                auditEntity.CreatedAt = DateTime.Now;
            }

            if (entry.Value == EntityState.Modified && auditEntity != null)
            {
                auditEntity.UpdatedAt = DateTime.Now;
            }
        }
    }
     
}
