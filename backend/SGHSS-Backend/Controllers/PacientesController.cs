// src/SGHSS_Backend/Controllers/PacientesController.cs
using Microsoft.AspNetCore.Mvc;
using SGHSS_Backend.DTOs.Pacientes;
using SGHSS_Backend.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Adicione este using

namespace SGHSS_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Rota base: /api/Pacientes
    [Authorize] // Protege todo o controlador: requer autenticação JWT para acesso
    public class PacientesController : ControllerSGHSS
    {
        private readonly PacienteService _pacienteService;

        public PacientesController(PacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        /// <summary>
        /// Lista todos os pacientes cadastrados.
        /// Requer autenticação e perfil 'ADMIN' ou 'PROFISSIONAL_SAUDE'.
        /// Endpoint: GET /api/Pacientes
        /// </summary>
        /// <returns>Uma lista de pacientes.</returns>
        [HttpGet]
        [Authorize(Roles = "ADMIN,PROFISSIONAL_SAUDE")] // Apenas ADMIN ou PROFISSIONAL_SAUDE podem listar
        public async Task<ActionResult<IEnumerable<PacienteResponse>>> GetPacientes()
        {
            var pacientes = await _pacienteService.GetAllPacientes();
            return Ok(pacientes);
        }

        /// <summary>
        /// Obtém um paciente pelo seu ID.
        /// Requer autenticação e perfil 'ADMIN', 'PROFISSIONAL_SAUDE' ou o próprio 'PACIENTE' (se o ID for o seu).
        /// Endpoint: GET /api/Pacientes/{id}
        /// </summary>
        /// <param name="id">ID do paciente.</param>
        /// <returns>Os dados do paciente.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,PROFISSIONAL_SAUDE,PACIENTE")] // ADMIN/PROFISSIONAL_SAUDE ou o próprio paciente
        public async Task<IActionResult> GetPacienteById(int id)
        {
            // Lógica de autorização para PACIENTE:
            // Um paciente só pode ver os próprios dados.
            // Para ADMIN/PROFISSIONAL_SAUDE, não há restrição de ID.
            if (User.IsInRole("PACIENTE"))
            {
                var usuarioIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(usuarioIdClaim, out int loggedInUserId))
                {
                    var pacienteDoUsuario = await _pacienteService.GetPacienteById(id);
                    if (pacienteDoUsuario == null || pacienteDoUsuario.IdUsuario != loggedInUserId)
                    {
                        return Forbid(); // Retorna 403 Forbidden se o paciente tentar acessar dados de outro
                    }
                }
                else
                {
                    return Unauthorized(new { message = "Usuário paciente não autenticado corretamente." });
                }
            }

            var paciente = await _pacienteService.GetPacienteById(id);

            if (paciente == null)
            {
                return NotFound(); // Retorna 404 Not Found
            }

            return Ok(paciente);
        }

        /// <summary>
        /// Atualiza os dados de um paciente existente.
        /// Requer autenticação e perfil 'ADMIN' ou 'PROFISSIONAL_SAUDE'.
        /// Endpoint: PUT /api/Pacientes/{id}
        /// </summary>
        /// <param name="id">ID do paciente a ser atualizado.</param>
        /// <param name="request">Novos dados do paciente.</param>
        /// <returns>O paciente atualizado.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,PROFISSIONAL_SAUDE")] // Apenas ADMIN ou PROFISSIONAL_SAUDE podem atualizar
        public async Task<IActionResult> PutPaciente(int id, [FromBody] PacienteUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedPaciente = await _pacienteService.UpdatePaciente(id, request);

            if (updatedPaciente == null)
            {
                return NotFound(); // Paciente não encontrado
            }

            return Ok(updatedPaciente);
        }

        /// <summary>
        /// Deleta um paciente pelo seu ID.
        /// Requer autenticação e perfil 'ADMIN'.
        /// Endpoint: DELETE /api/Pacientes/{id}
        /// </summary>
        /// <param name="id">ID do paciente a ser deletado.</param>
        /// <returns>Status de sucesso ou falha.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")] // Apenas ADMIN pode deletar
        public async Task<IActionResult> DeletePaciente(int id)
        {
            var isDeleted = await _pacienteService.DeletePaciente(id);

            if (!isDeleted)
            {
                return NotFound(); // Paciente não encontrado ou falha na exclusão
            }

            return NoContent(); // Retorna 204 No Content para indicar sucesso sem corpo de resposta
        }
    }
}