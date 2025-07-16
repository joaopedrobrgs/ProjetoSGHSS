using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SGHSS_Backend.Data;
using SGHSS_Backend.Data.Entities;
using SGHSS_Backend.DTOs.Pacientes;
using SGHSS_Backend.DTOs.Auth; // Para usar RegisterRequest no método de criação de paciente
using SGHSS_Backend.Services; // Para usar AuthService

namespace SGHSS_Backend.Services;

public class PacienteService
{
    private readonly SGHSSDbContext _context;
    private readonly AuthService _authService; // Para criar usuário associado ao paciente

    public PacienteService(SGHSSDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<IEnumerable<PacienteResponse>> GetAllPacientes()
    {
        return await _context.Pacientes
                             .Include(p => p.Usuario) // Inclui os dados do usuário associado
                             .Select(p => new PacienteResponse
                             {
                                 IdPaciente = p.IdPaciente,
                                 IdUsuario = p.IdUsuario,
                                 NomeCompleto = p.NomeCompleto,
                                 DataNascimento = p.DataNascimento,
                                 Cpf = p.Cpf,
                                 Telefone = p.Telefone,
                                 Endereco = p.Endereco,
                                 HistoricoClinico = p.HistoricoClinico,
                                 Rg = p.Rg,
                                 Sexo = p.Sexo,
                                 Convenio = p.Convenio,
                                 EmailUsuario = p.Usuario != null ? p.Usuario.Email : null // Pega o email do usuário se existir
                             })
                             .ToListAsync();
    }

    public async Task<PacienteResponse> GetPacienteById(int id)
    {
        var paciente = await _context.Pacientes
                                     .Include(p => p.Usuario)
                                     .FirstOrDefaultAsync(p => p.IdPaciente == id);

        if (paciente == null)
        {
            return null;
        }

        return new PacienteResponse
        {
            IdPaciente = paciente.IdPaciente,
            IdUsuario = paciente.IdUsuario,
            NomeCompleto = paciente.NomeCompleto,
            DataNascimento = paciente.DataNascimento,
            Cpf = paciente.Cpf,
            Telefone = paciente.Telefone,
            Endereco = paciente.Endereco,
            HistoricoClinico = paciente.HistoricoClinico,
            Rg = paciente.Rg,
            Sexo = paciente.Sexo,
            Convenio = paciente.Convenio,
            EmailUsuario = paciente.Usuario != null ? paciente.Usuario.Email : null
        };
    }

    public async Task<PacienteResponse> UpdatePaciente(int id, PacienteUpdateRequest request)
    {
        var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.IdPaciente == id);

        if (paciente == null)
        {
            return null; // Paciente não encontrado
        }

        // Atualiza apenas os campos que foram fornecidos na requisição (não nulos/vazios)
        if (!string.IsNullOrEmpty(request.NomeCompleto)) paciente.NomeCompleto = request.NomeCompleto;
        if (request.DataNascimento.HasValue) paciente.DataNascimento = request.DataNascimento.Value;
        if (!string.IsNullOrEmpty(request.Cpf)) paciente.Cpf = request.Cpf;
        if (!string.IsNullOrEmpty(request.Telefone)) paciente.Telefone = request.Telefone;
        if (!string.IsNullOrEmpty(request.Endereco)) paciente.Endereco = request.Endereco;
        if (!string.IsNullOrEmpty(request.HistoricoClinico)) paciente.HistoricoClinico = request.HistoricoClinico;
        if (!string.IsNullOrEmpty(request.Rg)) paciente.Rg = request.Rg;
        if (!string.IsNullOrEmpty(request.Sexo)) paciente.Sexo = request.Sexo;
        if (!string.IsNullOrEmpty(request.Convenio)) paciente.Convenio = request.Convenio;

        _context.Pacientes.Update(paciente);
        await _context.SaveChangesAsync();

        // Retorna o paciente atualizado, buscando o email do usuário se houver
        var updatedPaciente = await _context.Pacientes
                                            .Include(p => p.Usuario)
                                            .FirstOrDefaultAsync(p => p.IdPaciente == id);

        return new PacienteResponse
        {
            IdPaciente = updatedPaciente.IdPaciente,
            IdUsuario = updatedPaciente.IdUsuario,
            NomeCompleto = updatedPaciente.NomeCompleto,
            DataNascimento = updatedPaciente.DataNascimento,
            Cpf = updatedPaciente.Cpf,
            Telefone = updatedPaciente.Telefone,
            Endereco = updatedPaciente.Endereco,
            HistoricoClinico = updatedPaciente.HistoricoClinico,
            Rg = updatedPaciente.Rg,
            Sexo = updatedPaciente.Sexo,
            Convenio = updatedPaciente.Convenio,
            EmailUsuario = updatedPaciente.Usuario != null ? updatedPaciente.Usuario.Email : null
        };
    }

    public async Task<bool> DeletePaciente(int id)
    {
        var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.IdPaciente == id);

        if (paciente == null)
        {
            return false; // Paciente não encontrado
        }

        // Opcional: Se o paciente tiver um IdUsuario, você pode querer desativar/deletar o usuário também.
        // Para este projeto, vamos apenas deletar o paciente.
        // Se o relacionamento for Cascade Delete (no OnModelCreating), a exclusão do usuário
        // resultaria na exclusão do paciente. Aqui, focamos na exclusão do paciente.
        if (paciente.IdUsuario > 0)
        {
            var usuarioAssociado = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == paciente.IdUsuario);
            if (usuarioAssociado != null)
            {
                usuarioAssociado.Ativo = false; // Desativa o usuário em vez de deletar
                _context.Usuarios.Update(usuarioAssociado);
            }
        }


        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();
        return true; // Paciente deletado com sucesso
    }
}
