using System;
using System.Collections.Generic;

namespace SGHSS_Backend.Data.Entities;
public class Paciente
{
    public int IdPaciente { get; set; } // Corresponde a id_paciente no banco
    public int IdUsuario { get; set; } // Chave Estrangeira para Usuario
    public string NomeCompleto { get; set; }
    public DateTime DataNascimento { get; set; }
    public string Cpf { get; set; }
    public string Telefone { get; set; }
    public string Endereco { get; set; }
    public string HistoricoClinico { get; set; }
    public string Rg { get; set; }
    public string Sexo { get; set; }
    public string Convenio { get; set; }
    public string? Email { get; set; } // Email do paciente (nullable)

    // Propriedades de navegação
    public virtual Usuario Usuario { get; set; } // Relacionamento 1:1 ou 1:0 com Usuario
    public virtual ICollection<Consulta> Consultas { get; set; } // Um paciente pode ter muitas consultas
    public virtual ICollection<RelacaoProfissionalPaciente> Relacoes { get; set; } // Relacoes com profissionais
}
