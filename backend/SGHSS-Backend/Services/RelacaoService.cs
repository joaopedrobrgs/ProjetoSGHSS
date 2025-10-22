using Microsoft.EntityFrameworkCore;
using SGHSS_Backend.Data;
using SGHSS_Backend.Data.Entities;
using SGHSS_Backend.DTOs.Relacoes;
using SGHSS_Backend.Models.Exceptions;

namespace SGHSS_Backend.Services;

public class RelacaoService
{
    private readonly SGHSSDbContext _context;

    public RelacaoService(SGHSSDbContext context)
    {
        _context = context;
    }

    public async Task<RelacaoResponse> AtivarAsync(int idProfissional, int idPaciente, SGHSSDbContext? context = null)
    {
        context ??= _context;
        bool profOk = await context.Profissionais.AnyAsync(p => p.IdProfissional == idProfissional);
        if (!profOk) throw new CustomException("Profissional não encontrado.", 404);
        bool pacOk = await context.Pacientes.AnyAsync(p => p.IdPaciente == idPaciente);
        if (!pacOk) throw new CustomException("Paciente não encontrado.", 404);

        var rel = await context.RelacoesProfissionalPaciente.FirstOrDefaultAsync(r => r.IdProfissional == idProfissional && r.IdPaciente == idPaciente);
        if (rel == null)
        {
            rel = new RelacaoProfissionalPaciente
            {
                IdProfissional = idProfissional,
                IdPaciente = idPaciente,
                StatusRelacao = "Ativo"
            };
            context.RelacoesProfissionalPaciente.Add(rel);
        }
        else
        {
            rel.StatusRelacao = "Ativo";
            context.RelacoesProfissionalPaciente.Update(rel);
        }

        await context.SaveChangesAsync();
        return new RelacaoResponse
        {
            IdRelacao = rel.IdRelacao,
            IdProfissional = rel.IdProfissional,
            IdPaciente = rel.IdPaciente,
            StatusRelacao = rel.StatusRelacao
        };
    }

    public async Task<RelacaoResponse> InativarAsync(int idProfissional, int idPaciente, SGHSSDbContext? context = null)
    {
        context ??= _context;
        var rel = await context.RelacoesProfissionalPaciente.FirstOrDefaultAsync(r => r.IdProfissional == idProfissional && r.IdPaciente == idPaciente)
                  ?? throw new CustomException("Relação não encontrada.", 404);
        rel.StatusRelacao = "Inativo";
        context.RelacoesProfissionalPaciente.Update(rel);
        await context.SaveChangesAsync();
        return new RelacaoResponse
        {
            IdRelacao = rel.IdRelacao,
            IdProfissional = rel.IdProfissional,
            IdPaciente = rel.IdPaciente,
            StatusRelacao = rel.StatusRelacao
        };
    }
}
