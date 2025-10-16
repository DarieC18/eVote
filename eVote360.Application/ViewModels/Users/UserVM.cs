using System.ComponentModel.DataAnnotations;

namespace EVote360.Application.ViewModels.Users;

public class UserCreateVm
{
    [Required, StringLength(60)] public string FirstName { get; set; } = "";
    [Required, StringLength(60)] public string LastName { get; set; } = "";
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required, StringLength(40)] public string UserName { get; set; } = "";
    [Required, StringLength(64, MinimumLength = 6)] public string Password { get; set; } = "";
    [Required] public string Role { get; set; } = "Dirigente";
}

public class UserEditVm
{
    [Required] public int Id { get; set; }
    [Required, StringLength(60)] public string FirstName { get; set; } = "";
    [Required, StringLength(60)] public string LastName { get; set; } = "";
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required, StringLength(40)] public string UserName { get; set; } = "";
    [Required] public string Role { get; set; } = "Dirigente";
    [StringLength(64, MinimumLength = 6)] public string? NewPassword { get; set; }
}

