namespace EVote360.Application.ViewModels.Position;

public sealed class PositionListVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
