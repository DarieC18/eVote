using System.ComponentModel.DataAnnotations;

namespace EVote360.Application.ViewModels.Party;

public sealed class PartyFormVM
{
    [Required, StringLength(200)]
    public string Name { get; set; } = default!;

    [Required, StringLength(12)]
    public string Siglas { get; set; } = default!;

    [StringLength(400)]
    public string? Description { get; set; }

    public string? LogoPath { get; set; }

}
