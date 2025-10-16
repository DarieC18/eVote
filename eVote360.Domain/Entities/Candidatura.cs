using EVote360.Domain.Base;

namespace EVote360.Domain.Entities.Candidate;

public class Candidatura : AuditableEntity, ISoftDeletable
{
    public int Id { get; private set; }
    public Guid CandidateId { get; private set; }
    public Guid PositionId { get; private set; }
    public Guid PartyId { get; private set; }

    public bool IsActive { get; private set; } = true;

    private Candidatura() { }

    public Candidatura(Guid candidateId, Guid positionId, Guid partyId)
    {
        if (candidateId == Guid.Empty)
            throw new ArgumentException("CandidateId inválido.", nameof(candidateId));
        if (positionId == Guid.Empty)
            throw new ArgumentException("PositionId inválido.", nameof(positionId));
        if (partyId == Guid.Empty)
            throw new ArgumentException("PartyId inválido.", nameof(partyId));

        CandidateId = candidateId;
        PositionId = positionId;
        PartyId = partyId;
    }

    public void Activate() { if (!IsActive) { IsActive = true; Touch(); } }
    public void Deactivate() { if (IsActive) { IsActive = false; Touch(); } }
}
