using System.ComponentModel.DataAnnotations;

namespace SGHSS_Backend.DTOs.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string? Senha { get; set; }
}
