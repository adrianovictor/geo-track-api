using Core.Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoTruck.Services.Infrastructure.Mappings.Common;

public abstract class EntityMap<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : AuditableEntity<int>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(_ => _.CreatedAt).IsRequired().HasColumnType("datetime2");
        builder.Property(_ => _.UpdatedAt).HasColumnType("datetime2").IsRequired(false);

        Map(builder);
    }

    protected abstract void Map(EntityTypeBuilder<TEntity> builder);
}
