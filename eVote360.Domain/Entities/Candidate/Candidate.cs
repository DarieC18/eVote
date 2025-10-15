using EVote360.Domain.Base;

namespace EVote360.Domain.Entities.Candidate;

public class Candidate : AuditableEntity, ISoftDeletable
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Guid PartyId { get; private set; } // pertenencia primaria
    public bool IsActive { get; private set; } = true;

    private Candidate() { }

    public Candidate(string firstName, string lastName, Guid partyId)
    {
        SetName(firstName, lastName);
        SetParty(partyId);
    }

    public void SetName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("El nombre es requerido.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("El apellido es requerido.", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Touch();
    }

    public void SetParty(Guid partyId)
    {
        if (partyId == Guid.Empty)
            throw new ArgumentException("PartyId inválido.", nameof(partyId));
        PartyId = partyId;
        Touch();
    }

    public void Activate() { if (!IsActive) { IsActive = true; Touch(); } }
    public void Deactivate() { if (IsActive) { IsActive = false; Touch(); } }
}
