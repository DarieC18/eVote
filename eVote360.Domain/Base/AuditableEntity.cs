namespace EVote360.Domain.Base;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    protected void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}

