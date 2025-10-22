using Swashbuckle.AspNetCore.Filters;
using SGHSS_Backend.DTOs.Auth;
using System;

namespace SGHSS_Backend.Swagger.Examples.Auth;

public class RegisterPacienteRequestExample : IExamplesProvider<RegisterRequest>
{
    public RegisterRequest GetExamples()
    {
        return new RegisterRequest
        {
            Email = "paciente+demo@sghss.local",
            Senha = "Senha@123",
            Perfil = "PACIENTE",
            PacienteData = new PacienteDataRequest
            {
                NomeCompleto = "Paciente Demo",
                DataNascimento = new DateTime(1990, 5, 10),
                Cpf = "12345678909",
                Telefone = "11999999999",
                Endereco = "Rua X, 123",
                HistoricoClinico = "—",
                Rg = "RG123DEMO",
                Sexo = "M",
                Convenio = "Particular",
                EmailPaciente = "paciente.demo@mail.com"
            }
        };
    }
}
