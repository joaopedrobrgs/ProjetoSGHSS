using System.ComponentModel.DataAnnotations;

namespace SGHSS_Backend.DTOs.Profissionais;

public class ProfissionalUpdateRequest
{
    [StringLength(255)]
    public string? NomeCompleto { get; set; }

    [StringLength(14, ErrorMessage = "CPF deve ter até 14 caracteres com máscara.")]
    public string? Cpf { get; set; }

    [StringLength(20)]
    public string? Rg { get; set; }

    [StringLength(50)]
    public string? CrmOuConselho { get; set; }

    [StringLength(100)]
    public string? Especialidade { get; set; }

    [Phone]
    public string? Telefone { get; set; }

    [EmailAddress]
    [StringLength(255)]
    public string? EmailProfissional { get; set; }

    public string? DisponibilidadeAgenda { get; set; }
}