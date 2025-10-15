using EVote360.Domain.Base;
using EVote360.Domain.Enums;

namespace EVote360.Domain.Entities.Election;

public class Election : AuditableEntity
{
    public string Name { get; private set; }
    public DateTime ScheduledAt { get; private set; } // fecha nominal
    public ElectionState State { get; private set; } = ElectionState.Created;

    private Election() { }

    public Election(string name, DateTime scheduledAtUtc)
    {
        SetName(name);
        ScheduledAt = scheduledAtUtc;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de la elección es requerido.", nameof(name));
        Name = name.Trim();
        Touch();
    }

    public void Start()
    {
        if (State != ElectionState.Created)
            throw new InvalidOperationException("Solo elecciones en estado 'Created' pueden iniciar.");
        State = ElectionState.InProcess;
        Touch();
    }

    public void Finalize()
    {
        if (State != ElectionState.InProcess)
            throw new InvalidOperationException("Solo elecciones en proceso pueden finalizar.");
        State = ElectionState.Finalized;
        Touch();
    }
}
