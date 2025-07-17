using Microsoft.AspNetCore.Mvc;
using SGHSS_Backend.Data;
using SGHSS_Backend.DTOs.Auth;
using SGHSS_Backend.Models.Exceptions;
using SGHSS_Backend.Services;

namespace SGHSS_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerSGHSS
{
    private readonly SGHSSDbContext _context;
    private readonly AuthService _authService;

    public AuthController(AuthService authService, SGHSSDbContext context)
    {
        _context = context;
        _authService = authService;
    }

    /// <summary>
    /// Realiza o login de um usuário.
    /// Endpoint: POST /api/auth/login
    /// </summary>
    /// <param name="request">Dados de login (email e senha).</param>
    /// <returns>Token JWT se o login for bem-sucedido, ou erro de autenticação.</returns>
    [HttpPost("login")] // Define que este método responde a requisições POST para /api/auth/login
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                throw new CustomException("Parâmetros incorretos.");
            }
            var response = await _authService.Authenticate(request, _context) ?? throw new CustomException(null, 401);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return await CustomErrorRequestAsync(ex);
        }
    }

    /// <summary>
    /// Realiza o registro de um novo usuário.
    /// Endpoint: POST /api/auth/register
    /// </summary>
    /// <param name="request">Dados de registro (email, senha e perfil).</param>
    /// <returns>Dados do usuário registrado e token JWT, ou erro de validação/conflito.</returns>
    [HttpPost("register")] // Define que este método responde a requisições POST para /api/auth/register
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                throw new CustomException("Parâmetros incorretos.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            var response = await _authService.Register(request, _context) ?? throw new CustomException(null, 409);
            await transaction.CommitAsync();

            return StatusCode(201, response);
        }
        catch (Exception ex)
        {
            return await CustomErrorRequestAsync(ex);
        }
    }
}

