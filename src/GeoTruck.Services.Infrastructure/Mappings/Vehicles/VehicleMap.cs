using GeoTruck.Services.Domain.Entities;
using GeoTruck.Services.Domain.ValueOvject;
using GeoTruck.Services.Infrastructure.Mappings.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoTruck.Services.Infrastructure.Mappings.Vehicles;

public class VehicleMap : StatusableMap<Vehicle>
{
    protected override void Map(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");
        
        builder.HasKey(e => e.Id);
        builder.Property(e => e.UniqueId).IsRequired();
        builder.Property(e => e.Plate)
            .HasConversion(
                v => v.Value,
                v => new BrazilianPlate(v)
            )
            .IsRequired()
            .HasMaxLength(10);
        
        builder.Property(e => e.Renavam).IsRequired().HasMaxLength(20);
        builder.Property(e => e.Model).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Brand).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Year).IsRequired();

        builder.HasMany(e => e.Locations)
               .WithOne(l => l.Vehicle)
               .HasForeignKey(l => l.VehicleId);

        
    }
}
