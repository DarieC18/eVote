// eVote360.Infrastructure/Persistence/DataSeeder.cs
using EVote360.Domain.Entities;
using EVote360.Domain.Entities.Election;
using EVote360.Domain.Entities.Party;
using EVote360.Domain.Entities.Position;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        if (!await db.Usuarios.AnyAsync(ct))
        {
            var admin = new Usuario(
                nombre: "Admin",
                apellido: "Root",
                email: "admin@evote.local",
                nombreUsuario: "admin",
                passwordHash: "admin",
                rol: "Administrador");

            var dirigente = new Usuario(
                nombre: "Dina",
                apellido: "Rigente",
                email: "dirigente@evote.local",
                nombreUsuario: "dirigente",
                passwordHash: "dirigente",
                rol: "Dirigente");

            db.Usuarios.AddRange(admin, dirigente);
        }

        if (!await db.Parties.AnyAsync(ct))
        {
            db.Parties.AddRange(
                new Party("Partido Uno", "PU"),
                new Party("Partido Dos", "PD")
            );
        }

        if (!await db.Positions.AnyAsync(ct))
        {
            db.Positions.AddRange(
                new Position("Presidencia"),
                new Position("Senaduría")
            );
        }

        if (!await db.Elections.AnyAsync(ct))
        {
            var year = DateTime.UtcNow.Year;
            db.Elections.Add(new Election("Elección General", year));
        }

        await db.SaveChangesAsync(ct);
    }
}
