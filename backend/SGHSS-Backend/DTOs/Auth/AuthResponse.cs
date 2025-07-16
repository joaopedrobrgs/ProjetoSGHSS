namespace SGHSS_Backend.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; }
    public int IdUsuario { get; set; }
    public string Email { get; set; }
    public string Perfil { get; set; }
}
