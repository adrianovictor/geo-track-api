using GeoTruck.Services.Domain.Entities;

namespace GeoTruck.Services.Infrastructure.Extensions.Repositories;

public static class VehicleRepositoryExtensions
{
    public static IQueryable<Vehicle> ApplyFilters(
        this IQueryable<Vehicle> vehicles,
        string? renavam = null,
        string? plate = null,
        string? model = null,
        string? brand = null,
        int? year = null)
    {
        if (!string.IsNullOrEmpty(renavam))
            vehicles = vehicles.Where(v => v.Renavam.Contains(renavam));

        if (!string.IsNullOrEmpty(plate))
            vehicles = vehicles.Where(v => v.Plate.Value.Contains(plate));

        if (!string.IsNullOrEmpty(model))
            vehicles = vehicles.Where(v => v.Model.Contains(model));

        if (!string.IsNullOrEmpty(brand))
            vehicles = vehicles.Where(v => v.Brand.Contains(brand));

        if (year.HasValue)
            vehicles = vehicles.Where(v => v.Year == year.Value);

        vehicles = vehicles.Where(v => v.Status != Domain.Enum.Status.Delete);

        return vehicles;
    }

    /// <summary>
    /// Pula <paramref name="offset"/> registros e pega os <paramref name="limit"/> seguintes.
    /// O front pede <c>Offset = (página - 1) * Limit</c>. A ordem por Id garante que as páginas
    /// não se repitam nem pulem registros entre uma chamada e outra.
    /// </summary>
    public static IQueryable<Vehicle> Paginate(this IQueryable<Vehicle> query, int offset, int limit)
    {
        return query
            .OrderBy(v => v.Id)
            .Skip(offset)
            .Take(limit);
    }
}
