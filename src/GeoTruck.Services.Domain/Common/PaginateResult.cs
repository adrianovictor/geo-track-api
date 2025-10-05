namespace GeoTruck.Services.Domain.Common;

public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalRecords,
    int CurrentPage,
    int PageSize
);