// src/SGHSS_Backend/Controllers/PacientesController.cs
using Microsoft.AspNetCore.Mvc;
using SGHSS_Backend.DTOs.Pacientes;
using SGHSS_Backend.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SGHSS_Backend.Models.Exceptions;
using SGHSS_Backend.Data; // Adicione este using

namespace SGHSS_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Rota base: /api/Pacientes
    [Authorize] // Protege todo o controlador: requer autenticação JWT para acesso
    public class PacientesController : ControllerSGHSS
    {
        private readonly PacienteService _pacienteService;

        public PacientesController(SGHSSDbContext context, PacienteService pacienteService): base(context)
        {
            _pacienteService = pacienteService;
        }

        /// <summary>
        /// Lista todos os pacientes cadastrados.
        /// Requer autenticação e perfil 'ADMIN' ou 'PROFISSIONAL'.
        /// Endpoint: GET /api/Pacientes
        /// </summary>
        /// <returns>Uma lista de pacientes.</returns>
        [HttpGet]
        [Authorize(Roles = "ADMIN,PROFISSIONAL")] // Apenas ADMIN ou PROFISSIONAL podem listar
        public async Task<ActionResult<IEnumerable<PacienteResponse>>> GetPacientes()
        {
            try
            {
                var pacientes = await _pacienteService.GetAllPacientes();
                return Ok(pacientes);
            }
            catch (Exception ex)
            {
                return await CustomErrorRequestAsync(ex);
            }
        }

        /// <summary>
        /// Obtém um paciente pelo seu ID.
        /// Requer autenticação e perfil 'ADMIN', 'PROFISSIONAL_SAUDE' ou o próprio 'PACIENTE' (se o ID for o seu).
        /// Endpoint: GET /api/Pacientes/{id}
        /// </summary>
        /// <param name="id">ID do paciente.</param>
        /// <returns>Os dados do paciente.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,PROFISSIONAL,PACIENTE")] // ADMIN/PROFISSIONAL ou o próprio paciente
        public async Task<IActionResult> GetPacienteById(int id)
        {
            try
            {
                var user = await GetUserLoggedAsync() ?? throw new CustomException("Usuário não autenticado.", 401);
                var paciente = await _pacienteService.GetPacienteById(id, user) ?? throw new CustomException("Paciente não encontrado.", 404); // Exceção 404: Not Found
                return Ok(paciente);
            }
            catch (Exception ex)
            {
                return await CustomErrorRequestAsync(ex);
            }
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
        [Authorize(Roles = "ADMIN,PROFISSIONAL,PACIENTE")] // Apenas ADMIN ou PROFISSIONAL podem atualizar (Paciente vai atualizar apenas dados pessoais pela API de Usuário)
        public async Task<IActionResult> PutPaciente(int id, [FromBody] PacienteUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Parâmetros incorretos.");

                var user = await GetUserLoggedAsync() ?? throw new CustomException("Usuário não autenticado.", 401);

                await using var transaction = await _context.Database.BeginTransactionAsync();
                var updatedPaciente = await _pacienteService.UpdatePaciente(id, request, user) ?? throw new CustomException(null, 404); // Erro 404: Not Found (Paciente não encontrado)
                await transaction.CommitAsync();

                return Ok(updatedPaciente);
            }
            catch (Exception ex)
            {
                return await CustomErrorRequestAsync(ex);
            }
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
            try
            {
                var isDeleted = await _pacienteService.DeletePaciente(id);
                if (!isDeleted)
                    throw new CustomException(null, 404); // Erro 404: Not Found (Paciente não encontrado ou falha na exclusão)
                return NoContent(); // Retorna 204 No Content para indicar sucesso sem corpo de resposta
            }
            catch (Exception ex)
            {
                return await CustomErrorRequestAsync(ex);
            }
        }
    }
}