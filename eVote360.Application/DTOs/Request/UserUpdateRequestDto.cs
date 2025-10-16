namespace EVote360.Application.DTOs.Request;

public class UserUpdateRequestDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Role { get; set; } = "Dirigente";
    public string? NewPassword { get; set; }
}
