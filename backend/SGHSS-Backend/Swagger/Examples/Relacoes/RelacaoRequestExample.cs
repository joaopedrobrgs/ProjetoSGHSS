using Swashbuckle.AspNetCore.Filters;
using SGHSS_Backend.DTOs.Relacoes;

namespace SGHSS_Backend.Swagger.Examples.Relacoes;

public class RelacaoRequestExample : IExamplesProvider<RelacaoRequest>
{
    public RelacaoRequest GetExamples()
    {
        return new RelacaoRequest
        {
            IdProfissional = 1,
            IdPaciente = 1
        };
    }
}
