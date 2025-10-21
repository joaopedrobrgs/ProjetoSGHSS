using Microsoft.EntityFrameworkCore;
using SGHSS_Backend.Data;
using SGHSS_Backend.Data.Entities;
using SGHSS_Backend.DTOs.Profissionais;
using SGHSS_Backend.Models.Exceptions;
using System.Linq;

namespace SGHSS_Backend.Services;

public class ProfissionalService
{
    private readonly SGHSSDbContext _context;

    public ProfissionalService(SGHSSDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProfissionalResponse>> GetAllAsync(SGHSSDbContext? context = null)
    {
        context ??= _context;
        var list = await context.Profissionais
            .Include(p => p.Usuario)
            .Select(p => new ProfissionalResponse
            {
                IdProfissional = p.IdProfissional,
                IdUsuario = p.IdUsuario,
                NomeCompleto = p.NomeCompleto,
                Cpf = p.Cpf,
                Rg = p.Rg,
                CrmOuConselho = p.CrmOuConselho,
                Especialidade = p.Especialidade,
                Telefone = p.Telefone,
                EmailProfissional = p.EmailProfissional,
                DisponibilidadeAgenda = p.DisponibilidadeAgenda,
                EmailUsuario = p.Usuario.Email
            })
            .ToListAsync();
        return list;
    }

    public async Task<ProfissionalResponse> GetByIdAsync(int id, SGHSSDbContext? context = null)
    {
        context ??= _context;
        var p = await context.Profissionais.Include(p => p.Usuario).FirstOrDefaultAsync(p => p.IdProfissional == id)
            ?? throw new CustomException("Profissional não encontrado.", 404);
        return new ProfissionalResponse
        {
            IdProfissional = p.IdProfissional,
            IdUsuario = p.IdUsuario,
            NomeCompleto = p.NomeCompleto,
            Cpf = p.Cpf,
            Rg = p.Rg,
            CrmOuConselho = p.CrmOuConselho,
            Especialidade = p.Especialidade,
            Telefone = p.Telefone,
            EmailProfissional = p.EmailProfissional,
            DisponibilidadeAgenda = p.DisponibilidadeAgenda,
            EmailUsuario = p.Usuario.Email
        };
    }

    public async Task<ProfissionalResponse> UpdateAsync(int id, ProfissionalUpdateRequest request, Usuario user, SGHSSDbContext? context = null)
    {
        context ??= _context;
        var p = await context.Profissionais.FirstOrDefaultAsync(x => x.IdProfissional == id)
            ?? throw new CustomException("Profissional não encontrado.", 404);

        // PROFISSIONAL só pode editar o próprio cadastro
        if (user.Perfil == "PROFISSIONAL" && p.IdUsuario != user.IdUsuario)
            throw new CustomException("Usuário sem permissão para realizar essa ação.", 403);

        if (!string.IsNullOrEmpty(request.NomeCompleto)) p.NomeCompleto = request.NomeCompleto;
        if (!string.IsNullOrEmpty(request.Cpf))
        {
            var cpfLimpo = new string(request.Cpf.Where(char.IsDigit).ToArray());
            if (!Utils.Utils.ValidarCpf(cpfLimpo))
                throw new CustomException("Informe um CPF válido.", 400);
            bool cpfExists = await context.Profissionais.AnyAsync(x => x.IdProfissional != id && x.Cpf == cpfLimpo);
            if (cpfExists)
                throw new CustomException("Já existe profissional cadastrado com o mesmo CPF.", 409);
            p.Cpf = cpfLimpo;
        }

        if (!string.IsNullOrEmpty(request.Rg))
        {
            var rgLimpo = new string(request.Rg.Where(char.IsLetterOrDigit).ToArray());
            bool rgExists = await context.Profissionais.AnyAsync(x => x.IdProfissional != id && x.Rg == rgLimpo);
            if (rgExists)
                throw new CustomException("Já existe profissional cadastrado com o mesmo RG.", 409);
            p.Rg = rgLimpo;
        }

        if (!string.IsNullOrEmpty(request.CrmOuConselho))
        {
            // Garantir unicidade ao atualizar
            bool exists = await context.Profissionais.AnyAsync(x => x.CrmOuConselho == request.CrmOuConselho && x.IdProfissional != id);
            if (exists) throw new CustomException("Já existe um usuário cadastrado com o mesmo CRM/Conselho.", 409);
            p.CrmOuConselho = request.CrmOuConselho;
        }
        if (!string.IsNullOrEmpty(request.Especialidade)) p.Especialidade = request.Especialidade;
        if (!string.IsNullOrEmpty(request.Telefone)) p.Telefone = request.Telefone;
        if (!string.IsNullOrEmpty(request.EmailProfissional)) p.EmailProfissional = request.EmailProfissional;
        if (!string.IsNullOrEmpty(request.DisponibilidadeAgenda)) p.DisponibilidadeAgenda = request.DisponibilidadeAgenda;

        await context.SaveChangesAsync();

        var atualizado = await context.Profissionais.Include(x => x.Usuario).FirstAsync(x => x.IdProfissional == id);
        return new ProfissionalResponse
        {
            IdProfissional = atualizado.IdProfissional,
            IdUsuario = atualizado.IdUsuario,
            NomeCompleto = atualizado.NomeCompleto,
            Cpf = atualizado.Cpf,
            Rg = atualizado.Rg,
            CrmOuConselho = atualizado.CrmOuConselho,
            Especialidade = atualizado.Especialidade,
            Telefone = atualizado.Telefone,
            EmailProfissional = atualizado.EmailProfissional,
            DisponibilidadeAgenda = atualizado.DisponibilidadeAgenda,
            EmailUsuario = atualizado.Usuario.Email
        };
    }

    public async Task<bool> DeleteAsync(int id, SGHSSDbContext? context = null)
    {
        context ??= _context;
        var p = await context.Profissionais.FirstOrDefaultAsync(x => x.IdProfissional == id)
            ?? throw new CustomException(null, 404);

        // Verifica consultas vinculadas
        bool hasConsultas = await context.Consultas.AnyAsync(c => c.IdProfissional == id);
        if (hasConsultas)
            throw new CustomException("Não é possível excluir: há consultas vinculadas a este profissional.", 409);

        var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == p.IdUsuario)
            ?? throw new CustomException(null, 404);

        context.Usuarios.Remove(usuario);
        await context.SaveChangesAsync();
        return true;
    }
}
