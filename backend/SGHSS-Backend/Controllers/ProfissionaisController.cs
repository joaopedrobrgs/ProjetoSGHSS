using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGHSS_Backend.Data;
using SGHSS_Backend.DTOs.Profissionais;
using SGHSS_Backend.Models.Exceptions;
using SGHSS_Backend.Services;

namespace SGHSS_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfissionaisController : ControllerSGHSS
{
    private readonly ProfissionalService _service;

    public ProfissionaisController(SGHSSDbContext context, ProfissionalService service) : base(context)
    {
        _service = service;
    }

    /// <summary>
    /// Lista profissionais. Acesso: qualquer usuário autenticado.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProfissionais()
    {
        try
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }
        catch (Exception ex)
        {
            return await CustomErrorRequestAsync(ex);
        }
    }

    /// <summary>
    /// Detalhe por ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfissionalById(int id)
    {
        try
        {
            var p = await _service.GetByIdAsync(id);
            return Ok(p);
        }
        catch (Exception ex)
        {
            return await CustomErrorRequestAsync(ex);
        }
    }

    /// <summary>
    /// Atualiza dados do profissional. ADMIN pode atualizar qualquer; PROFISSIONAL apenas o próprio.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN,PROFISSIONAL")]
    public async Task<IActionResult> PutProfissional(int id, [FromBody] ProfissionalUpdateRequest request)
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

    /// <summary>
    /// Exclui profissional. Apenas ADMIN e sem consultas vinculadas.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteProfissional(int id)
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
