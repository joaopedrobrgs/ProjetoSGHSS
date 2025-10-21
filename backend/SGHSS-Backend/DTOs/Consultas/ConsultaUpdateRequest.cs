using System;
using System.ComponentModel.DataAnnotations;

namespace SGHSS_Backend.DTOs.Consultas;

public class ConsultaUpdateRequest
{
    [StringLength(100)]
    public string TipoConsulta { get; set; }

    [StringLength(50)]
    public string StatusConsulta { get; set; } // Agendada, Concluída, Cancelada

    [StringLength(500)]
    public string Observacoes { get; set; }

    public DateTime? DataHoraConsulta { get; set; }
}