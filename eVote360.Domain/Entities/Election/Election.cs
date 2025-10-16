using EVote360.Domain.Base;
using EVote360.Domain.Enums;

namespace EVote360.Domain.Entities.Election;

public sealed class Election : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public int Year { get; private set; }
    public ElectionStatus Status { get; private set; } = ElectionStatus.Draft;
    public DateTime? StartedAt { get; private set; }
    public DateTime? FinishedAt { get; private set; }

    private Election() { } // EF

    public Election(string name, int year)
    {
        SetName(name);
        SetYear(year);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre es requerido.", nameof(name));
        Name = name.Trim();
        Touch();
    }

    public void SetYear(int year)
    {
        if (year < 1900 || year > 3000)
            throw new ArgumentException("Año inválido.", nameof(year));
        Year = year;
        Touch();
    }

    public void Start()
    {
        if (Status != ElectionStatus.Draft)
            throw new InvalidOperationException("Solo se puede activar desde Borrador.");
        Status = ElectionStatus.Active;
        StartedAt = DateTime.UtcNow;
        Touch();
    }

    public void Finish()
    {
        if (Status != ElectionStatus.Active)
            throw new InvalidOperationException("Solo se puede finalizar si está Activa.");
        Status = ElectionStatus.Finished;
        FinishedAt = DateTime.UtcNow;
        Touch();
    }
}
