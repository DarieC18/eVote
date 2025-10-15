namespace EVote360.Domain.Base;

public interface ISoftDeletable
{
    bool IsActive { get; }
    void Activate();
    void Deactivate();
}
