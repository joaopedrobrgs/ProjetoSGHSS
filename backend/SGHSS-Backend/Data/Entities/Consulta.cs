using System;

namespace SGHSS_Backend.Data.Entities;

public class Consulta
{
    public int IdConsulta { get; set; } // Corresponde a id_consulta no banco
    public int IdPaciente { get; set; } // Chave Estrangeira para Paciente
    public int IdProfissional { get; set; } // Chave Estrangeira para Profissional
    public DateTime DataHoraConsulta { get; set; }
    public string TipoConsulta { get; set; }
    public string StatusConsulta { get; set; }
    public string Observacoes { get; set; }

    // Propriedades de navegação
    public virtual Paciente Paciente { get; set; } // Relacionamento com Paciente
    public virtual Profissional Profissional { get; set; } // Relacionamento com Profissional
}
