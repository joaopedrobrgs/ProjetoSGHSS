using Microsoft.EntityFrameworkCore;
using SGHSS_Backend.Data;
using SGHSS_Backend.Services;
using SGHSS_Backend.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SGHSS_Backend.Data.Seed;

namespace SGHSS_Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Configura o DbContext para usar SQL Server
            builder.Services.AddDbContext<SGHSSDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Registra os serviços personalizados
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<JwtTokenGenerator>();
            builder.Services.AddScoped<PacienteService>();
            builder.Services.AddScoped<ProfissionalService>();
            builder.Services.AddScoped<ConsultaService>();

            // Configura a autenticação JWT
            var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key (Jwt:Key) não configurada. Defina em appsettings.json, user-secrets ou variáveis de ambiente.");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            builder.Services.AddAuthorization(); // Habilita a autorização

            // CORS básico para consumo por front-ends locais
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", policy =>
                    policy
                        .WithOrigins(
                            "http://localhost:3000",
                            "http://localhost:5173",
                            "http://127.0.0.1:5173",
                            "http://127.0.0.1:3000"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                );
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("AllowLocalhost");

            app.MapControllers();

            // Migrate and seed
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SGHSSDbContext>();
                db.Database.Migrate();
                DbSeeder.SeedAdminAsync(db).GetAwaiter().GetResult();
            }

            app.Run();
        }
    }
}
