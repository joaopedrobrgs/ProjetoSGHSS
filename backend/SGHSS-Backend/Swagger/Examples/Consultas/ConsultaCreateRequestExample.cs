using Swashbuckle.AspNetCore.Filters;
using SGHSS_Backend.DTOs.Consultas;
using System;

namespace SGHSS_Backend.Swagger.Examples.Consultas;

public class ConsultaCreateRequestExample : IExamplesProvider<ConsultaCreateRequest>
{
    public ConsultaCreateRequest GetExamples()
    {
        return new ConsultaCreateRequest
        {
            IdPaciente = 1,
            IdProfissional = 1,
            DataHoraConsulta = DateTime.SpecifyKind(new DateTime(2025, 11, 20, 14, 0, 0), DateTimeKind.Utc),
            TipoConsulta = "Presencial",
            Observacoes = "Consulta de rotina"
        };
    }
}
