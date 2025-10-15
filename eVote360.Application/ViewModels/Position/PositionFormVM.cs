using System.ComponentModel.DataAnnotations;

namespace EVote360.Application.ViewModels.Position;

public sealed class PositionFormVM
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    [StringLength(120, ErrorMessage = "Máximo 120 caracteres.")]
    public string Name { get; set; } = default!;
}
