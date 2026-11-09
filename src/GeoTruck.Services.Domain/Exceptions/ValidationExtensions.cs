using System.Text.RegularExpressions;
using Core.Common.Domain.Exceptions;

namespace GeoTruck.Services.Domain.Exceptions;

/// <summary>
/// Validações de entrada. O nome do campo vai para a resposta da API (400, em <c>errors</c>),
/// então use o nome que o cliente envia, em camelCase.
/// </summary>
public static partial class ValidationExtensions
{
    public static void ThrowIfNullOrWhiteSpace(this string? value, string field, string? message = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationException(field, message ?? $"O campo '{field}' é obrigatório.");
        }
    }

    /// <summary>Erro de programação, não de entrada: continua sendo <see cref="ArgumentNullException"/> (500).</summary>
    public static void ThrowIfNull<T>(this T? value, string paramName, string? message = null) where T : class
    {
        if (value == null)
        {
            throw new ArgumentNullException(paramName, message ?? $"O parâmetro '{paramName}' não pode ser nulo.");
        }
    }

    public static void ThrowIfZeroOrNegative(this int value, string field, string? message = null)
    {
        if (value <= 0)
        {
            throw new ValidationException(field, message ?? $"O campo '{field}' deve ser maior que zero.");
        }
    }

    public static void ThrowIfValidYear(this int value, string field, string? message = null)
    {
        var currentYear = DateTime.UtcNow.Year;
        if (value < 1900 || value > currentYear)
        {
            throw new ValidationException(field, message ?? $"O campo '{field}' deve estar entre 1900 e {currentYear}.");
        }
    }

    public static void ThrowIfBrazilianVehiclePlateInvalid(this string plate, string field, string? message = null)
    {
        if (!OldPlate().IsMatch(plate) && !MercosulPlate().IsMatch(plate))
        {
            throw new ValidationException(field, message ?? $"O campo '{field}' não é uma placa brasileira válida (ABC1234, ABC-1234 ou ABC1D23).");
        }
    }

    [GeneratedRegex(@"^[A-Z]{3}-?[0-9]{4}$")]
    private static partial Regex OldPlate();

    [GeneratedRegex(@"^[A-Z]{3}[0-9][A-Z][0-9]{2}$")]
    private static partial Regex MercosulPlate();
}
