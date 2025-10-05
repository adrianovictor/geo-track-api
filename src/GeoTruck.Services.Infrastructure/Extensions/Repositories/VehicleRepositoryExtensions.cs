using GeoTruck.Services.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoTruck.Services.Infrastructure.Extensions.Repositories;

public static class VehicleRepositoryExtensions
{
    public static Task<Vehicle?> GetByIdAsync(this IQueryable<Vehicle> vehicles, int id)
    {
        return vehicles.FirstOrDefaultAsync(v => v.Id == id);
    }

    public static Task<Vehicle?> GetByLicensePlateAsync(this IQueryable<Vehicle> vehicles, string licensePlate) {
        return vehicles.FirstOrDefaultAsync(v => v.Plate == licensePlate);
    }

    public static Task<Vehicle> GetByRenavamAsync(this IQueryable<Vehicle> vehicles, string renavam) {
        return vehicles.SingleOrDefaultAsync(v => v.Renavam == renavam);
    }
}
