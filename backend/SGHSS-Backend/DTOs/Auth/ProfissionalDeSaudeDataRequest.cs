using System.ComponentModel.DataAnnotations;

namespace SGHSS_Backend.DTOs.Auth
{
    public class ProfissionalDataRequest
    {
        [Required(ErrorMessage = "O nome completo é obrigatório para um profissional de saúde.")]
        [StringLength(255, ErrorMessage = "O nome completo não pode exceder 255 caracteres.")]
    public string? NomeCompleto { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório para um profissional de saúde.")]
        [StringLength(14, ErrorMessage = "O CPF deve ter até 14 caracteres com máscara.")]
    public string? Cpf { get; set; }

        [Required(ErrorMessage = "O RG é obrigatório para um profissional de saúde.")]
        [StringLength(20, ErrorMessage = "O RG não pode exceder 20 caracteres.")]
    public string? Rg { get; set; }

        [Required(ErrorMessage = "O CRM/Conselho é obrigatório para um profissional de saúde.")]
        [StringLength(50, ErrorMessage = "O CRM/Conselho não pode exceder 50 caracteres.")]
    public string? CrmOuConselho { get; set; }

        [Required(ErrorMessage = "A especialidade é obrigatória para um profissional de saúde.")]
        [StringLength(100, ErrorMessage = "A especialidade não pode exceder 100 caracteres.")]
    public string? Especialidade { get; set; }

        [Phone(ErrorMessage = "Formato de telefone inválido.")]
    public string? Telefone { get; set; }

        [EmailAddress(ErrorMessage = "Formato de e-mail profissional inválido.")]
        [StringLength(255, ErrorMessage = "O e-mail profissional não pode exceder 255 caracteres.")]
    public string? EmailProfissional { get; set; }

    public string? DisponibilidadeAgenda { get; set; }
    }
}