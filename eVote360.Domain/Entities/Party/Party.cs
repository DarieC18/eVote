using EVote360.Domain.Base;

namespace EVote360.Domain.Entities.Party;

public sealed class Party : AuditableEntity, ISoftDeletable
{
    public string Name { get; private set; } = null!;
    public string Siglas { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    public string? LogoPath { get; private set; }

    private Party() { }

    public Party(string name, string siglas, string? description = null)
    {
        SetName(name);
        SetSiglas(siglas);
        SetDescription(description);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre es requerido.", nameof(name));

        Name = name.Trim();
        Touch();
    }

    public void SetSiglas(string siglas)
    {
        if (string.IsNullOrWhiteSpace(siglas))
            throw new ArgumentException("Las siglas son requeridas.", nameof(siglas));

        if (siglas.Length > 12)
            throw new ArgumentException("Las siglas no pueden exceder 12 caracteres.", nameof(siglas));

        Siglas = siglas.Trim().ToUpperInvariant();
        Touch();
    }

    public void SetDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Touch();
    }

    public void Activate()
    {
        if (!IsActive) { IsActive = true; Touch(); }
    }

    public void Deactivate()
    {
        if (IsActive) { IsActive = false; Touch(); }
    }
    public void SetLogoPath(string? path)
    {
        LogoPath = string.IsNullOrWhiteSpace(path) ? null : path.Trim();
        Touch();
    }
}
