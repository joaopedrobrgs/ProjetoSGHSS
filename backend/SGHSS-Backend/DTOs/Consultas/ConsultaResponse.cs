using System;

namespace SGHSS_Backend.DTOs.Consultas;

public class ConsultaResponse
{
    public int IdConsulta { get; set; }
    public int IdPaciente { get; set; }
    public int IdProfissional { get; set; }
    public DateTime DataHoraConsulta { get; set; }
    public string TipoConsulta { get; set; }
    public string StatusConsulta { get; set; }
    public string Observacoes { get; set; }

    // opcionais
    public string NomePaciente { get; set; }
    public string NomeProfissional { get; set; }
}
