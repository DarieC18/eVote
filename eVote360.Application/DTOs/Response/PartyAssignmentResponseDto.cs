namespace EVote360.Application.DTOs.PartyAssignments;

public class PartyAssignmentResponseDto
{
    public Guid Id { get; set; }
    public int UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = "";
    public Guid PartyId { get; set; }
    public string PartyNombre { get; set; } = "";
    public bool Activo { get; set; }
}
