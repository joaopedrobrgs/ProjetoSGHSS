using Swashbuckle.AspNetCore.Filters;
using SGHSS_Backend.DTOs.Consultas;
using System;

namespace SGHSS_Backend.Swagger.Examples.Consultas;

public class ConsultaUpdateRequestExample : IExamplesProvider<ConsultaUpdateRequest>
{
    public ConsultaUpdateRequest GetExamples()
    {
        return new ConsultaUpdateRequest
        {
            DataHoraConsulta = DateTime.SpecifyKind(new DateTime(2025, 11, 20, 15, 0, 0), DateTimeKind.Utc),
            TipoConsulta = "Retorno",
            StatusConsulta = "Remarcada",
            Observacoes = "Ajuste de horário"
        };
    }
}
