using System;

namespace SGHSS_Backend.DTOs.Pacientes;

public class PacienteResponse
{
    public int IdPaciente { get; set; }
    public int IdUsuario { get; set; } // O ID do usuário associado
    public string NomeCompleto { get; set; }
    public DateTime DataNascimento { get; set; }
    public string Cpf { get; set; }
    public string Telefone { get; set; }
    public string Endereco { get; set; }
    public string HistoricoClinico { get; set; }
    public string Rg { get; set; }
    public string Sexo { get; set; }
    public string Convenio { get; set; }
    public string EmailUsuario { get; set; } // Adiciona o email do usuário associado
}

