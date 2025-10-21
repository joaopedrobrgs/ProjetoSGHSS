using Microsoft.EntityFrameworkCore;
using SGHSS_Backend.Data.Entities;
using SGHSS_Backend.Utils;

namespace SGHSS_Backend.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(SGHSSDbContext db, CancellationToken ct = default)
    {
        // Ajuste aqui o e-mail/senha padrão do ADMIN conforme preferir
        const string adminEmail = "admin@sghss.local";
        const string adminSenhaPlano = "Admin@123"; // Trocar depois por segredo seguro

        var exists = await db.Usuarios.AsNoTracking().AnyAsync(u => u.Email == adminEmail, ct);
        if (exists) return;

        var admin = new Usuario
        {
            Email = adminEmail,
            Senha = PasswordHasher.HashPassword(adminSenhaPlano),
            Perfil = "ADMIN",
            DataCriacao = DateTime.UtcNow,
            Ativo = true
        };

        db.Usuarios.Add(admin);
        await db.SaveChangesAsync(ct);
    }
}
