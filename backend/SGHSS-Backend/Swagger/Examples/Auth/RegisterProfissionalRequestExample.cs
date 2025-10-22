using Swashbuckle.AspNetCore.Filters;
using SGHSS_Backend.DTOs.Auth;

namespace SGHSS_Backend.Swagger.Examples.Auth;

public class RegisterProfissionalRequestExample : IExamplesProvider<RegisterRequest>
{
    public RegisterRequest GetExamples()
    {
        return new RegisterRequest
        {
            Email = "prof+demo@sghss.local",
            Senha = "Senha@123",
            Perfil = "PROFISSIONAL",
            ProfissionalData = new ProfissionalDataRequest
            {
                NomeCompleto = "Dr. Demo",
                Cpf = "98765432100",
                Rg = "PRGDEMO1",
                CrmOuConselho = "CRM-DEMO-1234",
                Especialidade = "Clínico Geral",
                Telefone = "11988888888",
                EmailProfissional = "prof.demo@mail.com",
                DisponibilidadeAgenda = "Seg-Sex 9-17h"
            }
        };
    }
}
