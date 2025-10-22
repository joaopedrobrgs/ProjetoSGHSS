using Swashbuckle.AspNetCore.Filters;
using SGHSS_Backend.DTOs.Pacientes;
using System;

namespace SGHSS_Backend.Swagger.Examples.Pacientes;

public class PacienteUpdateRequestExample : IExamplesProvider<PacienteUpdateRequest>
{
    public PacienteUpdateRequest GetExamples()
    {
        return new PacienteUpdateRequest
        {
            NomeCompleto = "Paciente Atualizado",
            DataNascimento = new DateTime(2000, 1, 1),
            Cpf = "12345678909",
            Telefone = "11911111112",
            Endereco = "Rua A, 101",
            HistoricoClinico = "N/A",
            Rg = "RG123ATU",
            Sexo = "M",
            Convenio = "Unimed",
            EmailPaciente = "paciente.atualizado@dominio.com"
        };
    }
}
