using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGHSS_Backend.Data;
using SGHSS_Backend.DTOs.Consultas;
using SGHSS_Backend.Models.Exceptions;
using SGHSS_Backend.Services;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;

namespace SGHSS_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConsultasController : ControllerSGHSS
{
    private readonly ConsultaService _service;

    public ConsultasController(SGHSSDbContext context, ConsultaService service) : base(context)
    {
        _service = service;
    }

    /// <summary>
    /// Cria uma consulta. PACIENTE agenda para si; PROFISSIONAL na própria agenda; ADMIN para qualquer.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [SwaggerRequestExample(typeof(ConsultaCreateRequest), typeof(SGHSS_Backend.Swagger.Examples.Consultas.ConsultaCreateRequestExample))]
    public async Task<IActionResult> Create([FromBody] ConsultaCreateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                throw new Exception("Parâmetros incorretos.");

            var user = await GetUserLoggedAsync();
            var result = await _service.CreateAsync(request, user);
            return StatusCode(201, result);
        }
        catch (Exception ex)
        {
            return await CustomErrorRequestAsync(ex);
        }
    }

    /// <summary>
    /// Lista consultas de acordo com o perfil: ADMIN todas, PROFISSIONAL só as suas, PACIENTE só as suas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyConsultas()
    {
        try
        {
            var user = await GetUserLoggedAsync();
            var list = await _service.GetAllForUserAsync(user);
            return Ok(list);
        }
        catch (Exception ex)
        {
            return await CustomErrorRequestAsync(ex);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var user = await GetUserLoggedAsync();
            var c = await _service.GetByIdAsync(id, user);
            return Ok(c);
        }
        catch (Exception ex)
        {
            return await CustomErrorRequestAsync(ex);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(ConsultaUpdateRequest), typeof(SGHSS_Backend.Swagger.Examples.Consultas.ConsultaUpdateRequestExample))]
    public async Task<IActionResult> Update(int id, [FromBody] ConsultaUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                throw new Exception("Parâmetros incorretos.");

            var user = await GetUserLoggedAsync();
            var updated = await _service.UpdateAsync(id, request, user);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return await CustomErrorRequestAsync(ex);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) throw new CustomException(null, 404);
            return NoContent();
        }
        catch (Exception ex)
        {
            return await CustomErrorRequestAsync(ex);
        }
    }
}
