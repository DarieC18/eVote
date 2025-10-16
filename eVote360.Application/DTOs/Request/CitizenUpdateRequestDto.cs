namespace EVote360.Application.DTOs.Request;

public class CitizenUpdateRequestDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string NationalId { get; set; } = "";
}
