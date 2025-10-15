namespace EVote360.Application.DTOs.Response;

public sealed class PositionResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
