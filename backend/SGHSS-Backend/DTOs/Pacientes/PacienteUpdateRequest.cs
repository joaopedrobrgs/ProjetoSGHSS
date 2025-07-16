using System;
using System.ComponentModel.DataAnnotations;

namespace SGHSS_Backend.DTOs.Pacientes;

public class PacienteUpdateRequest
{
    // ID do paciente será passado na URL, não no corpo
    // Não é obrigatório ter todos os campos para atualização parcial (PATCH),
    // mas para PUT, todos os campos não-chave devem estar presentes.

    [StringLength(255, ErrorMessage = "O nome completo não pode exceder 255 caracteres.")]
    public string NomeCompleto { get; set; }

    [DataType(DataType.Date, ErrorMessage = "Formato de data de nascimento inválido.")]
    public DateTime? DataNascimento { get; set; } // Nullable para atualização parcial

    [StringLength(14, MinimumLength = 11, ErrorMessage = "O CPF deve ter entre 11 e 14 caracteres.")]
    public string Cpf { get; set; }

    [Phone(ErrorMessage = "Formato de telefone inválido.")]
    public string Telefone { get; set; }

    [StringLength(500, ErrorMessage = "O endereço não pode exceder 500 caracteres.")]
    public string Endereco { get; set; }

    public string HistoricoClinico { get; set; }

    [StringLength(20, ErrorMessage = "O RG não pode exceder 20 caracteres.")]
    public string Rg { get; set; }

    [StringLength(10, ErrorMessage = "O sexo não pode exceder 10 caracteres.")]
    public string Sexo { get; set; }

    [StringLength(100, ErrorMessage = "O convênio não pode exceder 100 caracteres.")]
    public string Convenio { get; set; }
}
