namespace EVote360.Application.DTOs.Request;

public sealed class PartyCreateRequest
{
    public string Name { get; set; } = default!;
    public string Siglas { get; set; } = default!;
    public string? Description { get; set; }
    public string? LogoPath { get; set; }  // lo setea el controller después de subir el archivo
}
