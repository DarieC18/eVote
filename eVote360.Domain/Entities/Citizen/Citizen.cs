using EVote360.Domain.Base;
using EVote360.Domain.ValueObjects;

namespace EVote360.Domain.Entities.Citizen;

public class Citizen : AuditableEntity, ISoftDeletable
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public NationalId NationalId { get; private set; }
    public EmailAddress? Email { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Citizen() { }

    public Citizen(string firstName, string lastName, NationalId nationalId, EmailAddress? email = null)
    {
        SetName(firstName, lastName);
        NationalId = nationalId ?? throw new ArgumentNullException(nameof(nationalId));
        Email = email;
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

    public void SetEmail(EmailAddress? email)
    {
        Email = email; // null permitido
        Touch();
    }

    public void Activate() { if (!IsActive) { IsActive = true; Touch(); } }
    public void Deactivate() { if (IsActive) { IsActive = false; Touch(); } }
}
