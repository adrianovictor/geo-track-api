using System;

namespace GeoTruck.Services.Domain.Exceptions;

public class CannotChangeStatusOfADeletedEntityException : Exception
{
    public CannotChangeStatusOfADeletedEntityException() : base("Não é possível mudar o status de uma entidade deletada.")
    {
    }
}
