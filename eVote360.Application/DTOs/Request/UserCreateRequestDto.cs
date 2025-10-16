namespace EVote360.Application.DTOs.Request;

public class UserCreateRequestDto
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Password { get; set; } = ""; //(se hashea en el servicio)
    public string Role { get; set; } = "Dirigente"; //
}
