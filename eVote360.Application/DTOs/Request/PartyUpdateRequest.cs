namespace EVote360.Application.DTOs.Request;

public sealed class PartyUpdateRequest
{
    public string Name { get; set; } = default!;
    public string Siglas { get; set; } = default!;
    public string? Description { get; set; }
    public string? LogoPath { get; set; }
}
