using GeoTruck.Services.Domain.Entities;
using GeoTruck.Services.Infrastructure.Mappings.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoTruck.Services.Infrastructure.Mappings.Vehicles;

public class VehicleLocationMap : EntityMap<VehicleLocation>
{
    protected override void Map(EntityTypeBuilder<VehicleLocation> builder)
    {
        builder.ToTable("VehicleLocations");
        
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Latitude).IsRequired();
        builder.Property(e => e.Longitude).IsRequired();
        builder.Property(e => e.Date).IsRequired().HasColumnType("datetime2");
        builder.Property(e => e.DateUTC).IsRequired();
    }
}
