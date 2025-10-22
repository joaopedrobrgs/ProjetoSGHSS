using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGHSS_Backend.Data;
using SGHSS_Backend.DTOs.Relacoes;
using SGHSS_Backend.Models.Exceptions;
using SGHSS_Backend.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;

namespace SGHSS_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")] // Apenas ADMIN pode gerenciar relações manualmente
public class RelacoesController : ControllerSGHSS
{
    private readonly RelacaoService _service;

    public RelacoesController(SGHSSDbContext context, RelacaoService service) : base(context)
    {
        _service = service;
    }

    /// <summary>
    /// Ativa (ou cria) a relação entre um profissional e um paciente.
    /// </summary>
    [HttpPost("ativar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [SwaggerRequestExample(typeof(RelacaoRequest), typeof(SGHSS_Backend.Swagger.Examples.Relacoes.RelacaoRequestExample))]
    public async Task<IActionResult> Ativar([FromBody] RelacaoRequest request)
    {
        try
        {
            if (!ModelState.IsValid) throw new CustomException("Parâmetros incorretos.", 400);
            var result = await _service.AtivarAsync(request.IdProfissional, request.IdPaciente);
            return Ok(result);
        }
        catch (System.Exception ex)
        {
            return await CustomErrorRequestAsync(ex);
        }
    }

    /// <summary>
    /// Inativa a relação entre um profissional e um paciente.
    /// </summary>
    [HttpPut("inativar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [SwaggerRequestExample(typeof(RelacaoRequest), typeof(SGHSS_Backend.Swagger.Examples.Relacoes.RelacaoRequestExample))]
    public async Task<IActionResult> Inativar([FromBody] RelacaoRequest request)
    {
        try
        {
            if (!ModelState.IsValid) throw new CustomException("Parâmetros incorretos.", 400);
            var result = await _service.InativarAsync(request.IdProfissional, request.IdPaciente);
            return Ok(result);
        }
        catch (System.Exception ex)
        {
            return await CustomErrorRequestAsync(ex);
        }
    }
}
