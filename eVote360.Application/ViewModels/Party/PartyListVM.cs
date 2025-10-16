namespace EVote360.Application.ViewModels.Party;

public sealed class PartyListVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Siglas { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? LogoPath { get; set; }
}
