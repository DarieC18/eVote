using EVote360.Domain.Base;

namespace EVote360.Domain.Entities.Position;

public class Position : AuditableEntity, ISoftDeletable
{
    public string Name { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Position() { }

    public Position(string name)
    {
        SetName(name);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del puesto es requerido.", nameof(name));

        Name = name.Trim();
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
}
