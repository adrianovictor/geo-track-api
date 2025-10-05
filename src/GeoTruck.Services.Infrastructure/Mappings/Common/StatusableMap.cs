using GeoTruck.Services.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoTruck.Services.Infrastructure.Mappings.Common;

public abstract class StatusableMap<TEntity> : EntityMap<TEntity>
    where TEntity : Statusable<TEntity>, IEntity<TEntity>
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(_ => _.Status).IsRequired().HasColumnName("Status");

        builder.Ignore(_ => _.IsInactive);
        builder.Ignore(_ => _.IsActive);
        builder.Ignore(_ => _.IsDeleted);
        builder.Ignore(_ => _.IsBlocked);        

        base.Configure(builder);
    }
}
