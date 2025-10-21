using System;
using SGHSS_Backend.Data.Entities;
using SGHSS_Backend.Data;
using SGHSS_Backend.DTOs.Auth;
using SGHSS_Backend.Utils;
using Microsoft.EntityFrameworkCore;
using SGHSS_Backend.Models.Exceptions;

namespace SGHSS_Backend.Services;

public class AuthService
{
    private readonly SGHSSDbContext _context;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public AuthService(SGHSSDbContext context, JwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> Authenticate(LoginRequest request, SGHSSDbContext? context = null)
    {
        context ??= _context;
        try
        {
            // 1. Encontrar o usuário pelo email
            var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email) ?? throw new Exception("Email e/ou senha inválido(s). Verifique seus dados e tente novamente.");

            // 2. Verificar a senha (compare o hash da senha)
            // IMPORTANTE: Em um sistema real, você usaria uma biblioteca segura para hash/verificação de senha (ex: BCrypt.Net)
            if (!PasswordHasher.VerifyPassword(request.Senha, usuario.Senha))
            {
                throw new Exception("Email e/ou senha inválido(s). Verifique seus dados e tente novamente."); // Senha incorreta
            }

            // 3. Gerar token JWT
            var token = _jwtTokenGenerator.GenerateToken(usuario.IdUsuario, usuario.Email, usuario.Perfil);

            return new AuthResponse
            {
                Token = token,
                IdUsuario = usuario.IdUsuario,
                Email = usuario.Email,
                Perfil = usuario.Perfil
            };
        }
        catch
        {
            throw;
        }
    }

    public async Task<AuthResponse> Register(RegisterRequest request, SGHSSDbContext? context = null)
    {
        context ??= _context;
        try
        {
            // 1. Verificar se o email já está em uso
            var emailExistente = await context.Usuarios.AnyAsync(u => u.Email == request.Email);
            if (emailExistente)
                throw new CustomException("Email informado já está em uso.");

            // 2. Hash da senha antes de salvar
            var hashedPassword = PasswordHasher.HashPassword(request.Senha);

            // 3. Criar novo usuário
            var novoUsuario = new Usuario
            {
                Email = request.Email,
                Senha = hashedPassword,
                Perfil = request.Perfil,
                DataCriacao = DateTime.UtcNow,
                Ativo = true
            };

            context.Usuarios.Add(novoUsuario);
            await context.SaveChangesAsync(); // Salvar o usuário para que ele receba um Id

            // 4. Lógica condicional para criar Paciente ou Profissional de Saúde
            if (request.Perfil == "PACIENTE")
            {
                if (request.PacienteData is null)
                    throw new CustomException("Informe os dados do paciente.");

                // Verificar se o CPF já está cadastrado para outro paciente
                var cpfExistente = await context.Pacientes.AnyAsync(p => p.Cpf == request.PacienteData.Cpf);
                if (cpfExistente)
                    throw new CustomException("Já existe um usuário cadastrado com o mesmo CPF.");

                var novoPaciente = new Paciente
                {
                    IdUsuario = novoUsuario.IdUsuario,
                    NomeCompleto = request.PacienteData.NomeCompleto,
                    DataNascimento = request.PacienteData.DataNascimento,
                    Cpf = request.PacienteData.Cpf,
                    Telefone = request.PacienteData.Telefone,
                    Endereco = request.PacienteData.Endereco,
                    HistoricoClinico = request.PacienteData.HistoricoClinico,
                    Rg = request.PacienteData.Rg,
                    Sexo = request.PacienteData.Sexo,
                    Convenio = request.PacienteData.Convenio
                };
                context.Pacientes.Add(novoPaciente);
                await context.SaveChangesAsync();
            }
            else if (request.Perfil == "PROFISSIONAL")
            {
                if (request.ProfissionalData == null)
                    throw new CustomException("Informe os dados do profissional.");

                // Verificar se o CRM/Conselho já está cadastrado
                var crmExistente = await context.Profissionais.AnyAsync(ps => ps.CrmOuConselho == request.ProfissionalData.CrmOuConselho);
                if (crmExistente)
                    throw new CustomException("Já existe um usuário cadastrado com o mesmo CRM/Conselho.");

                var novoProfissional = new Profissional
                {
                    IdUsuario = novoUsuario.IdUsuario,
                    NomeCompleto = request.ProfissionalData.NomeCompleto,
                    CrmOuConselho = request.ProfissionalData.CrmOuConselho,
                    Especialidade = request.ProfissionalData.Especialidade,
                    Telefone = request.ProfissionalData.Telefone,
                    EmailProfissional = request.ProfissionalData.EmailProfissional,
                    DisponibilidadeAgenda = request.ProfissionalData.DisponibilidadeAgenda
                };
                context.Profissionais.Add(novoProfissional);
                await context.SaveChangesAsync();
            }
            // Para 'ADMIN', não há dados extras a serem salvos.

            // 5. Gerar token JWT para o novo usuário
            var token = _jwtTokenGenerator.GenerateToken(novoUsuario.IdUsuario, novoUsuario.Email, novoUsuario.Perfil);

            return new AuthResponse
            {
                Token = token,
                IdUsuario = novoUsuario.IdUsuario,
                Email = novoUsuario.Email,
                Perfil = novoUsuario.Perfil
            };
        }
        catch
        {
            throw;
        }
    }
}
