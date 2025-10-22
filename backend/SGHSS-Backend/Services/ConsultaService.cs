using Microsoft.EntityFrameworkCore;
using SGHSS_Backend.Data;
using SGHSS_Backend.Data.Entities;
using SGHSS_Backend.DTOs.Consultas;
using SGHSS_Backend.Models.Exceptions;

namespace SGHSS_Backend.Services;

public class ConsultaService
{
    private readonly SGHSSDbContext _context;

    public ConsultaService(SGHSSDbContext context)
    {
        _context = context;
    }

    public async Task<ConsultaResponse> CreateAsync(ConsultaCreateRequest request, Usuario user, SGHSSDbContext? context = null)
    {
        context ??= _context;

        // Regras de acesso:
        if (user.Perfil == "PACIENTE")
        {
            var paciente = await context.Pacientes.FirstOrDefaultAsync(p => p.IdUsuario == user.IdUsuario)
                ?? throw new CustomException("Paciente não encontrado para o usuário.", 404);
            if (paciente.IdPaciente != request.IdPaciente)
                throw new CustomException("Usuário sem permissão para agendar consulta para outro paciente.", 403);
        }
        else if (user.Perfil == "PROFISSIONAL")
        {
            var prof = await context.Profissionais.FirstOrDefaultAsync(p => p.IdUsuario == user.IdUsuario)
                ?? throw new CustomException("Profissional não encontrado para o usuário.", 404);
            if (prof.IdProfissional != request.IdProfissional)
                throw new CustomException("Profissional só pode agendar em sua própria agenda.", 403);
        }

        // Validar existência das FKs
        var pacienteOk = await context.Pacientes.AnyAsync(p => p.IdPaciente == request.IdPaciente);
        if (!pacienteOk) throw new CustomException("Paciente informado não existe.", 404);
        var profOk = await context.Profissionais.AnyAsync(p => p.IdProfissional == request.IdProfissional);
        if (!profOk) throw new CustomException("Profissional informado não existe.", 404);

        // Checa conflito de horário para o mesmo profissional
        bool conflito = await context.Consultas.AnyAsync(c => c.IdProfissional == request.IdProfissional && c.DataHoraConsulta == request.DataHoraConsulta);
        if (conflito) throw new CustomException("Horário já ocupado para este profissional.", 409);

        var nova = new Consulta
        {
            IdPaciente = request.IdPaciente,
            IdProfissional = request.IdProfissional,
            DataHoraConsulta = request.DataHoraConsulta,
            TipoConsulta = request.TipoConsulta,
            StatusConsulta = "Agendada",
            Observacoes = request.Observacoes
        };

        context.Consultas.Add(nova);
        await context.SaveChangesAsync();

        // Garante/ativa relação Profissional-Paciente
        var rel = await context.RelacoesProfissionalPaciente.FirstOrDefaultAsync(r =>
            r.IdProfissional == request.IdProfissional && r.IdPaciente == request.IdPaciente);
        if (rel == null)
        {
            rel = new RelacaoProfissionalPaciente
            {
                IdProfissional = request.IdProfissional,
                IdPaciente = request.IdPaciente,
                StatusRelacao = "Ativo"
            };
            context.RelacoesProfissionalPaciente.Add(rel);
            await context.SaveChangesAsync();
        }
        else if (!string.Equals(rel.StatusRelacao, "Ativo", StringComparison.OrdinalIgnoreCase))
        {
            rel.StatusRelacao = "Ativo";
            context.RelacoesProfissionalPaciente.Update(rel);
            await context.SaveChangesAsync();
        }

        var result = await context.Consultas
            .Include(c => c.Paciente)
            .Include(c => c.Profissional)
            .FirstAsync(c => c.IdConsulta == nova.IdConsulta);

        return new ConsultaResponse
        {
            IdConsulta = result.IdConsulta,
            IdPaciente = result.IdPaciente,
            IdProfissional = result.IdProfissional,
            DataHoraConsulta = result.DataHoraConsulta,
            TipoConsulta = result.TipoConsulta,
            StatusConsulta = result.StatusConsulta,
            Observacoes = result.Observacoes,
            NomePaciente = result.Paciente.NomeCompleto,
            NomeProfissional = result.Profissional.NomeCompleto
        };
    }

    public async Task<IEnumerable<ConsultaResponse>> GetAllForUserAsync(Usuario user, SGHSSDbContext? context = null)
    {
        context ??= _context;
        IQueryable<Consulta> query = context.Consultas
            .Include(c => c.Paciente)
            .Include(c => c.Profissional);

        if (user.Perfil == "PACIENTE")
        {
            var paciente = await context.Pacientes.FirstOrDefaultAsync(p => p.IdUsuario == user.IdUsuario)
                ?? throw new CustomException("Paciente não encontrado para o usuário.", 404);
            query = query.Where(c => c.IdPaciente == paciente.IdPaciente);
        }
        else if (user.Perfil == "PROFISSIONAL")
        {
            var prof = await context.Profissionais.FirstOrDefaultAsync(p => p.IdUsuario == user.IdUsuario)
                ?? throw new CustomException("Profissional não encontrado para o usuário.", 404);
            query = query.Where(c => c.IdProfissional == prof.IdProfissional);
        }
        // ADMIN vê todas

        var list = await query
            .OrderBy(c => c.DataHoraConsulta)
            .Select(c => new ConsultaResponse
            {
                IdConsulta = c.IdConsulta,
                IdPaciente = c.IdPaciente,
                IdProfissional = c.IdProfissional,
                DataHoraConsulta = c.DataHoraConsulta,
                TipoConsulta = c.TipoConsulta,
                StatusConsulta = c.StatusConsulta,
                Observacoes = c.Observacoes,
                NomePaciente = c.Paciente.NomeCompleto,
                NomeProfissional = c.Profissional.NomeCompleto
            })
            .ToListAsync();
        return list;
    }

    public async Task<ConsultaResponse> GetByIdAsync(int id, Usuario user, SGHSSDbContext? context = null)
    {
        context ??= _context;
        var c = await context.Consultas
            .Include(x => x.Paciente)
            .Include(x => x.Profissional)
            .FirstOrDefaultAsync(x => x.IdConsulta == id) ?? throw new CustomException("Consulta não encontrada.", 404);

        if (user.Perfil == "PACIENTE")
        {
            var paciente = await context.Pacientes.FirstOrDefaultAsync(p => p.IdUsuario == user.IdUsuario) ?? throw new CustomException(null, 404);
            if (c.IdPaciente != paciente.IdPaciente) throw new CustomException("Sem permissão.", 403);
        }
        else if (user.Perfil == "PROFISSIONAL")
        {
            var prof = await context.Profissionais.FirstOrDefaultAsync(p => p.IdUsuario == user.IdUsuario) ?? throw new CustomException(null, 404);
            if (c.IdProfissional != prof.IdProfissional) throw new CustomException("Sem permissão.", 403);
        }

        return new ConsultaResponse
        {
            IdConsulta = c.IdConsulta,
            IdPaciente = c.IdPaciente,
            IdProfissional = c.IdProfissional,
            DataHoraConsulta = c.DataHoraConsulta,
            TipoConsulta = c.TipoConsulta,
            StatusConsulta = c.StatusConsulta,
            Observacoes = c.Observacoes,
            NomePaciente = c.Paciente.NomeCompleto,
            NomeProfissional = c.Profissional.NomeCompleto
        };
    }

    public async Task<ConsultaResponse> UpdateAsync(int id, ConsultaUpdateRequest request, Usuario user, SGHSSDbContext? context = null)
    {
        context ??= _context;
        var c = await context.Consultas.FirstOrDefaultAsync(x => x.IdConsulta == id) ?? throw new CustomException("Consulta não encontrada.", 404);

        if (user.Perfil == "PACIENTE")
        {
            var paciente = await context.Pacientes.FirstOrDefaultAsync(p => p.IdUsuario == user.IdUsuario) ?? throw new CustomException(null, 404);
            if (c.IdPaciente != paciente.IdPaciente) throw new CustomException("Sem permissão.", 403);
        }
        else if (user.Perfil == "PROFISSIONAL")
        {
            var prof = await context.Profissionais.FirstOrDefaultAsync(p => p.IdUsuario == user.IdUsuario) ?? throw new CustomException(null, 404);
            if (c.IdProfissional != prof.IdProfissional) throw new CustomException("Sem permissão.", 403);
        }

        if (!string.IsNullOrEmpty(request.TipoConsulta)) c.TipoConsulta = request.TipoConsulta;
        if (!string.IsNullOrEmpty(request.Observacoes)) c.Observacoes = request.Observacoes;
        if (request.DataHoraConsulta.HasValue)
        {
            // Evita conflito para mesmo profissional
            bool conflito = await context.Consultas.AnyAsync(x => x.IdProfissional == c.IdProfissional && x.IdConsulta != id && x.DataHoraConsulta == request.DataHoraConsulta.Value);
            if (conflito) throw new CustomException("Horário já ocupado para este profissional.", 409);
            c.DataHoraConsulta = request.DataHoraConsulta.Value;
        }

        if (!string.IsNullOrEmpty(request.StatusConsulta))
        {
            var novoStatus = request.StatusConsulta;
            if (user.Perfil == "PACIENTE" && novoStatus != "Cancelada")
                throw new CustomException("Paciente só pode cancelar a própria consulta.", 403);
            c.StatusConsulta = novoStatus;
        }

        context.Consultas.Update(c);
        await context.SaveChangesAsync();

        var r = await context.Consultas.Include(x => x.Paciente).Include(x => x.Profissional).FirstAsync(x => x.IdConsulta == id);
        return new ConsultaResponse
        {
            IdConsulta = r.IdConsulta,
            IdPaciente = r.IdPaciente,
            IdProfissional = r.IdProfissional,
            DataHoraConsulta = r.DataHoraConsulta,
            TipoConsulta = r.TipoConsulta,
            StatusConsulta = r.StatusConsulta,
            Observacoes = r.Observacoes,
            NomePaciente = r.Paciente.NomeCompleto,
            NomeProfissional = r.Profissional.NomeCompleto
        };
    }

    public async Task<bool> DeleteAsync(int id, SGHSSDbContext? context = null)
    {
        context ??= _context;
        var c = await context.Consultas.FirstOrDefaultAsync(x => x.IdConsulta == id) ?? throw new CustomException(null, 404);
        context.Consultas.Remove(c);
        await context.SaveChangesAsync();
        return true;
    }
}
