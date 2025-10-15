using EVote360.Domain.Base;

namespace EVote360.Domain.Entities.Party;

public class Party : AuditableEntity, ISoftDeletable
{
    public string Name { get; private set; }
    public string Siglas { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Party() { }

    public Party(string name, string siglas, string? description = null)
    {
        SetName(name);
        SetSiglas(siglas);
        Description = description?.Trim();
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del partido es requerido.", nameof(name));
        Name = name.Trim();
        Touch();
    }

    public void SetSiglas(string siglas)
    {
        if (string.IsNullOrWhiteSpace(siglas))
            throw new ArgumentException("Las siglas son requeridas.", nameof(siglas));
        Siglas = siglas.Trim().ToUpperInvariant();
        Touch();
    }

    public void SetDescription(string? description)
    {
        Description = description?.Trim();
        Touch();
    }

    public void Activate() { if (!IsActive) { IsActive = true; Touch(); } }
    public void Deactivate() { if (IsActive) { IsActive = false; Touch(); } }
}
