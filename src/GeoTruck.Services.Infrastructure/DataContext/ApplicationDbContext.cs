using Core.Common.Domain.Entities;
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
            if (entry.Key is not IAuditable)
            {
                continue;
            }

            // As datas têm set protegido na entidade: quem grava é a persistência, pelo EF.
            // Horário local (DateTime.Now) mantido de propósito, para não misturar com os registros antigos.
            var entityEntry = Entry(entry.Key);

            if (entry.Value == EntityState.Added)
            {
                entityEntry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = DateTime.Now;
            }

            if (entry.Value == EntityState.Modified)
            {
                entityEntry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue = DateTime.Now;
            }
        }
    }

}
