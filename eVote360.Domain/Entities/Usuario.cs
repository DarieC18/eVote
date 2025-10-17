namespace EVote360.Domain.Entities;

using EVote360.Domain.Entities.Assignments;

public class Usuario
{
    public int Id { get; private set; }

    public string Nombre { get; private set; } = "";
    public string Apellido { get; private set; } = "";
    public string Email { get; private set; } = "";

    public string NombreUsuario { get; private set; } = "";
    public string PasswordHash { get; private set; } = "";
    public string Rol { get; private set; } = "Dirigente"; // "Administrador" | "Dirigente"
    public bool Activo { get; private set; } = true;

    public ICollection<PartyAssignment> PartyAssignments { get; private set; }
        = new List<PartyAssignment>();

    public string FullName => $"{Nombre} {Apellido}";

    private Usuario() { }

    public Usuario(string nombre, string apellido, string email, string nombreUsuario, string passwordHash, string rol)
    {
        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        NombreUsuario = nombreUsuario;
        PasswordHash = passwordHash;
        Rol = rol;
        Activo = true;
    }

    public void Activar() => Activo = true;
    public void Desactivar() => Activo = false;

    public void SetNombre(string nombre, string apellido)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre requerido");
        if (string.IsNullOrWhiteSpace(apellido)) throw new ArgumentException("Apellido requerido");
        Nombre = nombre.Trim();
        Apellido = apellido.Trim();
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email requerido");
        Email = email.Trim();
    }

    public void SetNombreUsuario(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Usuario requerido");
        NombreUsuario = userName.Trim();
    }

    public void SetRol(string rol)
    {
        if (string.IsNullOrWhiteSpace(rol)) throw new ArgumentException("Rol requerido");
        Rol = rol.Trim(); // "Administrador" | "Dirigente"
    }

    public void SetPasswordHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash)) throw new ArgumentException("Hash requerido");
        PasswordHash = hash;
    }

}
