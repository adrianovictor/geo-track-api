using System;

namespace GeoTruck.Services.Domain.Exceptions;

public static class ValidationExtensions
{
    public static void ThrowIfNullOrWhiteSpace(this string? value, string paramName, string? message = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(paramName, message ?? $"O parâmetro '{paramName}' não pode ser nulo ou vazio.");
        }
    }

    public static void ThrowIfNull<T>(this T? value, string paramName, string? message = null) where T : class
    {
        if (value == null)
        {
            throw new ArgumentNullException(paramName, message ?? $"O parâmetro '{paramName}' não pode ser nulo.");
        }
    }

    public static void ThrowIfNull<T>(this T? value, string paramName, string? message = null) where T : struct
    {
        if (!value.HasValue)
        {
            throw new ArgumentNullException(paramName, message ?? $"O parâmetro '{paramName}' não pode ser nulo.");
        }
    }

    public static void ThrowIfZeroOrNegative(this int value, string paramName, string? message = null)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, message ?? $"O parâmetro '{paramName}' deve ser maior que zero.");
        }
    }

    public static void ThrowIfValidYear(this int value, string paramName, string? message = null)
    {
        var currentYear = DateTime.UtcNow.Year;
        if (value < 1900 || value > currentYear)
        {
            throw new ArgumentOutOfRangeException(paramName, message ?? $"O parâmetro '{paramName}' deve estar entre 1900 e {currentYear}.");
        }
    }

    public static void ThrowIfNegative(this decimal value, string paramName, string? message = null)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, message ?? $"O parâmetro '{paramName}' não pode ser negativo.");
        }
    }

    public static void ThrowIfBrazilianVehiclePlateInvalid(this string plate, string paramName, string? message = null)
    {
        var regexOld = new System.Text.RegularExpressions.Regex("^[A-Z]{3}-[0-9]{4}$");
        var regexNew = new System.Text.RegularExpressions.Regex("^[A-Z]{3}[0-9][A-Z][0-9]{2}$");

        if (!regexOld.IsMatch(plate) && !regexNew.IsMatch(plate))
        {
            throw new ArgumentException(message ?? $"O parâmetro '{paramName}' não é uma placa de veículo brasileira válida.", paramName);
        }
    }  
}
