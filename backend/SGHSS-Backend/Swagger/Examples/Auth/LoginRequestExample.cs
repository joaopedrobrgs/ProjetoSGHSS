using Swashbuckle.AspNetCore.Filters;
using SGHSS_Backend.DTOs.Auth;

namespace SGHSS_Backend.Swagger.Examples.Auth;

public class LoginRequestExample : IExamplesProvider<LoginRequest>
{
    public LoginRequest GetExamples()
    {
        return new LoginRequest
        {
            Email = "admin@sghss.local",
            Senha = "Admin@123"
        };
    }
}
