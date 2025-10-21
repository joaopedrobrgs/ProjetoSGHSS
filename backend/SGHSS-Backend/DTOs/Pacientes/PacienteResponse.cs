using System;

namespace SGHSS_Backend.DTOs.Pacientes;

public class PacienteResponse
{
    public int IdPaciente { get; set; }
    public int IdUsuario { get; set; } // O ID do usuário associado
    public required string NomeCompleto { get; set; }
    public DateTime DataNascimento { get; set; }
    public required string Cpf { get; set; }
    public required string Telefone { get; set; }
    public required string Endereco { get; set; }
    public required string HistoricoClinico { get; set; }
    public required string Rg { get; set; }
    public required string Sexo { get; set; }
    public required string Convenio { get; set; }
    public string? EmailPaciente { get; set; }
    public required string EmailUsuario { get; set; } // Adiciona o email do usuário associado
}

