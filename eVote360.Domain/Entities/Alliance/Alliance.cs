using EVote360.Domain.Base;
using EVote360.Domain.Enums;

namespace EVote360.Domain.Entities.Alliance;

public class Alliance : AuditableEntity, ISoftDeletable
{
    public Guid PartyAId { get; private set; }
    public Guid PartyBId { get; private set; }
    public AllianceStatus Status { get; private set; } = AllianceStatus.Pending;
    public DateTime RequestedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Alliance() { }

    public Alliance(Guid partyAId, Guid partyBId)
    {
        if (partyAId == Guid.Empty || partyBId == Guid.Empty)
            throw new ArgumentException("Partidos inválidos para alianza.");
        if (partyAId == partyBId)
            throw new ArgumentException("No se puede crear alianza con el mismo partido.");

        PartyAId = partyAId;
        PartyBId = partyBId;
    }

    public void Accept()
    {
        EnsurePending();
        Status = AllianceStatus.Accepted;
        RespondedAt = DateTime.UtcNow;
        Touch();
    }

    public void Reject()
    {
        EnsurePending();
        Status = AllianceStatus.Rejected;
        RespondedAt = DateTime.UtcNow;
        Touch();
    }

    public void Cancel()
    {
        if (Status is AllianceStatus.Accepted or AllianceStatus.Rejected or AllianceStatus.Cancelled)
            throw new InvalidOperationException("La alianza ya tiene estado final.");

        Status = AllianceStatus.Cancelled;
        RespondedAt = DateTime.UtcNow;
        Touch();
    }

    public void Activate() { if (!IsActive) { IsActive = true; Touch(); } }
    public void Deactivate() { if (IsActive) { IsActive = false; Touch(); } }

    private void EnsurePending()
    {
        if (Status != AllianceStatus.Pending)
            throw new InvalidOperationException("Solo alianzas pendientes pueden cambiar a aceptadas/rechazadas.");
    }
}

//extensión para detectar estados finales:
file static class AllianceStatusExtensions
{
    public static bool Finally(this AllianceStatus status) =>
        status is AllianceStatus.Accepted or AllianceStatus.Rejected or AllianceStatus.Cancelled;
}

