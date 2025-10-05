using System;

namespace GeoTruck.Services.Domain.Common;

public interface IEntity<in TEntity> : IAuditing
    where TEntity : class
{
    int Id { get; }
    bool IsPersisted();
}
