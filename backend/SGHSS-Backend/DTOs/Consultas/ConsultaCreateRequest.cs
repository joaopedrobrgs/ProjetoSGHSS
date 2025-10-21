using System;
using System.ComponentModel.DataAnnotations;

namespace SGHSS_Backend.DTOs.Consultas;

public class ConsultaCreateRequest
{
    [Required]
    public int IdPaciente { get; set; }

    [Required]
    public int IdProfissional { get; set; }

    [Required]
    public DateTime DataHoraConsulta { get; set; }

    [Required]
    [StringLength(100)]
    public string TipoConsulta { get; set; }

    [StringLength(500)]
    public string Observacoes { get; set; }
}