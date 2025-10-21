// src/SGHSS_Backend/DTOs/Auth/RegisterRequest.cs
using System.ComponentModel.DataAnnotations;

namespace SGHSS_Backend.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    public string? Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    public string? Senha { get; set; }

        [Required(ErrorMessage = "O perfil é obrigatório.")]
        [RegularExpression("^(PACIENTE|PROFISSIONAL|ADMIN)$", ErrorMessage = "Perfil inválido. Use PACIENTE, PROFISSIONAL ou ADMIN.")]
    public string? Perfil { get; set; } // "PACIENTE", "PROFISSIONAL", "ADMIN"

        // Dados opcionais para Paciente
        public PacienteDataRequest? PacienteData { get; set; }

        // Dados opcionais para Profissional
        public ProfissionalDataRequest? ProfissionalData { get; set; }
    }
}