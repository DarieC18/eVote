using EVote360.Domain.Base;

namespace EVote360.Domain.Entities.Assignments;

//asignacin de un usuario (sistema) a un partido concreto.
///UserId es un Guid de la capa de identidad (fuera de Domain).

public class PartyAssignment : AuditableEntity
{
    public Guid UserId { get; private set; }
    public Guid PartyId { get; private set; }

    private PartyAssignment() { }

    public PartyAssignment(Guid userId, Guid partyId)
    {
        if (userId == Guid.Empty || partyId == Guid.Empty)
            throw new ArgumentException("UserId o PartyId inválido.");
        UserId = userId;
        PartyId = partyId;
    }
}

