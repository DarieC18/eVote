namespace EVote360.Domain.Base;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    protected BaseEntity() { }
}

