using EVote360.Domain.Base;
using EVote360.Domain.Entities;
using PartyEntity = EVote360.Domain.Entities.Party.Party;


namespace EVote360.Domain.Entities.Assignments;

public class PartyAssignment : AuditableEntity
{
    public int UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; } = default!;
    public Guid PartyId { get; private set; }
    public PartyEntity Party { get; private set; } = default!;
    public bool Activo { get; private set; } = true;

    private PartyAssignment() { }

    public PartyAssignment(int usuarioId, Guid partyId)
    {
        if (usuarioId <= 0) throw new ArgumentException("UsuarioId inválido.", nameof(usuarioId));
        if (partyId == Guid.Empty) throw new ArgumentException("PartyId inválido.", nameof(partyId));

        UsuarioId = usuarioId;
        PartyId = partyId;
        Activo = true;
    }

    public void Activar() => Activo = true;
    public void Desactivar() => Activo = false;
}
