namespace GeoTruck.Services.Domain.Common;

public interface IAuditing
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}
