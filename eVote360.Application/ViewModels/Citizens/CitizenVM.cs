using System.ComponentModel.DataAnnotations;

namespace EVote360.Application.ViewModels.Citizens;

public class CitizenVm
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string NationalId { get; set; } = ""; // cédula
    public bool IsActive { get; set; }
}

public class CitizenCreateVm
{
    [Required, StringLength(60)]
    public string FirstName { get; set; } = "";

    [Required, StringLength(60)]
    public string LastName { get; set; } = "";

    [Required, EmailAddress, StringLength(120)]
    public string Email { get; set; } = "";

    [Required, StringLength(20, MinimumLength = 5, ErrorMessage = "Cédula inválida")]
    public string NationalId { get; set; } = "";
}

public class CitizenEditVm
{
    [Required]
    public Guid Id { get; set; }

    [Required, StringLength(60)]
    public string FirstName { get; set; } = "";

    [Required, StringLength(60)]
    public string LastName { get; set; } = "";

    [Required, EmailAddress, StringLength(120)]
    public string Email { get; set; } = "";

    [Required, StringLength(20, MinimumLength = 5)]
    public string NationalId { get; set; } = "";
}
