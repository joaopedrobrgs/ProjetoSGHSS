using System;
using Microsoft.EntityFrameworkCore;
using SGHSS_Backend.Data;
using SGHSS_Backend.Data.Entities;
using SGHSS_Backend.DTOs.Pacientes;
using SGHSS_Backend.Models.Exceptions; // Exceções customizadas

namespace SGHSS_Backend.Services;

public class PacienteService
{
    private readonly SGHSSDbContext _context;

    public PacienteService(SGHSSDbContext context) // Injete no construtor
    {
        _context = context;
    }

    public async Task<IEnumerable<PacienteResponse>> GetAllPacientes(SGHSSDbContext? context = null)
    {
        context ??= _context;
        return await context.Pacientes
                             .AsNoTracking()
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
                                 EmailUsuario = p.Usuario.Email // Pega o email do usuário se existir
                             })
                             .ToListAsync();
    }

    public async Task<PacienteResponse> GetPacienteById(int id, Usuario user, SGHSSDbContext? context = null)
    {
        context ??= _context;
        var paciente = await context.Pacientes
                                     .AsNoTracking()
                                     .Include(p => p.Usuario)
                                     .FirstOrDefaultAsync(p => p.IdPaciente == id)
                                     ?? throw new CustomException("Paciente não encontrado.", 404); // Erro 404: Not Found

        // Lógica de autorização para PACIENTE:
        // Um paciente só pode ver os próprios dados.
        // Para ADMIN/PROFISSIONAL_SAUDE, não há restrição de ID.
        if (user.Perfil == "PACIENTE" && paciente.IdUsuario != user.IdUsuario)
            throw new CustomException("Usuário sem permissão para realizar essa ação.", 403); // Erro 403: Forbidden

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
            EmailUsuario = paciente.Usuario.Email
        };
    }

    public async Task<PacienteResponse> UpdatePaciente(int id, PacienteUpdateRequest request, Usuario user, SGHSSDbContext? context = null)
    {
        context ??= _context;
        var paciente = await context.Pacientes.FirstOrDefaultAsync(p => p.IdPaciente == id)
                         ?? throw new CustomException("Paciente não encontrado.", 404);

        if (user.Perfil == "PACIENTE" && paciente.IdUsuario != user.IdUsuario)
            throw new CustomException("Usuário sem permissão para realizar essa ação.", 403);

        // Atualiza apenas os campos que foram fornecidos na requisição (não nulos/vazios)
        if (!string.IsNullOrEmpty(request.NomeCompleto)) paciente.NomeCompleto = request.NomeCompleto;
        if (request.DataNascimento.HasValue) paciente.DataNascimento = request.DataNascimento.Value;
        if (!string.IsNullOrEmpty(request.Cpf)) paciente.Cpf = new string(request.Cpf.Where(char.IsDigit).ToArray());
        if (await context.Pacientes.AnyAsync(p => p.IdPaciente != paciente.IdPaciente && p.Cpf == paciente.Cpf))
            throw new CustomException("Já existe paciente cadastrado com o mesmo CPF.", 409);
        if (!Utils.Utils.ValidarCpf(paciente.Cpf))
            throw new CustomException("Informe um CPF válido.", 400);
        if (!string.IsNullOrEmpty(request.Telefone)) paciente.Telefone = request.Telefone;
        if (!string.IsNullOrEmpty(request.Endereco)) paciente.Endereco = request.Endereco;
        if (!string.IsNullOrEmpty(request.Rg)) paciente.Rg = new string(request.Rg.Where(char.IsLetterOrDigit).ToArray());
        if (await context.Pacientes.AnyAsync(p => p.IdPaciente != paciente.IdPaciente && p.Rg == paciente.Rg))
            throw new CustomException("Já existe paciente cadastrado com o mesmo RG.", 409);
        if (!string.IsNullOrEmpty(request.Sexo)) paciente.Sexo = request.Sexo;
        if (!string.IsNullOrEmpty(request.HistoricoClinico))
        {
            if (user.Perfil == "PACIENTE")
                throw new CustomException("Paciente não tem permissão para alterar o próprio histórico clínico.", 403);
            paciente.HistoricoClinico = request.HistoricoClinico;
        }
        if (!string.IsNullOrEmpty(request.Convenio))
        {
            if (user.Perfil == "PACIENTE")
                throw new CustomException("Paciente não tem permissão para alterar a informação sobre o convênio médico.", 403);
            paciente.Convenio = request.Convenio;
        }

        await context.SaveChangesAsync();

        // Retorna o paciente atualizado, buscando o email do usuário se houver
        var updatedPaciente = await context.Pacientes
                                            .AsNoTracking()
                                            .Include(p => p.Usuario)
                                            .FirstOrDefaultAsync(p => p.IdPaciente == id)
                                            ?? throw new CustomException("Paciente não encontrado.", 404);

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
            EmailUsuario = updatedPaciente.Usuario.Email
        };
    }

    public async Task<bool> DeletePaciente(int id, SGHSSDbContext? context = null)
    {
        context ??= _context;
        var paciente = await context.Pacientes.FirstOrDefaultAsync(p => p.IdPaciente == id)
                         ?? throw new CustomException("Paciente não encontrado.", 404);

        // Deleta usuário do Paciente e, por consequência, o próprio paciente (cascade):
        var usuarioAssociado = await context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == paciente.IdUsuario)
                                ?? throw new CustomException("Usuário associado não encontrado.", 404);

        context.Usuarios.Remove(usuarioAssociado);
        await context.SaveChangesAsync();

        return true;
    }
}
