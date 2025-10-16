namespace EVote360.Application.DTOs.Response;

public class PartyResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Acronym { get; set; } = "";
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string? LogoPath { get; set; }
}
