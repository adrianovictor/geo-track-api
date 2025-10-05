using GeoTruck.Services.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoTruck.Services.Infrastructure.Extensions.Repositories;

public static class VehicleRepositoryExtensions
{
    public static Task<Vehicle?> GetByIdAsync(this IQueryable<Vehicle> vehicles, int id)
    {
        return vehicles.FirstOrDefaultAsync(v => v.Id == id);
    }

    public static Task<Vehicle?> GetByLicensePlateAsync(this IQueryable<Vehicle> vehicles, string licensePlate)
    {
        return vehicles.FirstOrDefaultAsync(v => v.Plate == licensePlate);
    }

    public static Task<Vehicle?> GetByRenavamAsync(this IQueryable<Vehicle> vehicles, string renavam)
    {
        return vehicles.SingleOrDefaultAsync(v => v.Renavam == renavam);
    }
    
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
            vehicles = vehicles.Where(v => v.Plate.Contains(plate));

        if (!string.IsNullOrEmpty(model))
            vehicles = vehicles.Where(v => v.Model.Contains(model));

        if (!string.IsNullOrEmpty(brand))
            vehicles = vehicles.Where(v => v.Brand.Contains(brand));

        if (year.HasValue)
            vehicles = vehicles.Where(v => v.Year == year.Value);

        return vehicles;
    }

    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int offset, int limit)
    {
        return query
            .Skip(Math.Max(0, offset - 1))
            .Take(Math.Max(1, limit));
    }    
}
