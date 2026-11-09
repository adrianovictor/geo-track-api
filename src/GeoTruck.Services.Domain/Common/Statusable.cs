using Core.Common.Domain.Entities;
using GeoTruck.Services.Domain.Enum;
using GeoTruck.Services.Domain.Exceptions;

namespace GeoTruck.Services.Domain.Common;

/// <summary>
/// Entidade com status (ativo, inativo, bloqueado, excluído). A exclusão é lógica: muda o status.
/// Fica no geo-track, e não na lib, porque os valores de status são do banco deste projeto.
/// </summary>
public abstract class Statusable : AuditableEntity<int>
{
    public Status Status { get; set; }

    public bool IsActive => Status == Status.Active;
    public bool IsInactive => Status == Status.Inactive;
    public bool IsBlocked => Status == Status.Blocked;
    public bool IsDeleted => Status == Status.Delete;

    public virtual void ChangeStatus(Status status)
    {
        if (IsDeleted)
        {
            throw new CannotChangeStatusOfADeletedEntityException();
        }

        Status = status;
    }

    public virtual void Activate()
    {
        ChangeStatus(Status.Active);
    }
    public virtual void Inactivate()
    {
        ChangeStatus(Status.Inactive);
    }
    public virtual void Block()
    {
        ChangeStatus(Status.Blocked);
    }
}
