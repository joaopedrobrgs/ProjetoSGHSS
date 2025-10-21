using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SGHSS_Backend.Utils;

public class JwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(int userId, string email, string profile)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var keyString = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key (Jwt:Key) não configurada.");
        var key = Encoding.ASCII.GetBytes(keyString);
        var issuer = _configuration["Jwt:Issuer"] ?? "SGHSS_API";
        var audience = _configuration["Jwt:Audience"] ?? "SGHSS_Users";
        var expiresConfig = _configuration["Jwt:ExpiresInMinutes"];
        var expiresInMinutes = !string.IsNullOrWhiteSpace(expiresConfig) && double.TryParse(expiresConfig, out var m) ? m : 60d;

        var claims = new ClaimsIdentity(new Claim[]
        {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, profile) // O perfil do usuário
        });

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = issuer,
            Audience = audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
