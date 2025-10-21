using System;
using SGHSS_Backend.Data.Entities;
using SGHSS_Backend.Data;
using SGHSS_Backend.DTOs.Auth;
using SGHSS_Backend.Utils;
using System.Linq;
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
                var cpfLimpo = request.PacienteData.Cpf != null ? new string(request.PacienteData.Cpf.Where(char.IsDigit).ToArray()) : null;
                var rgLimpo = request.PacienteData.Rg != null ? new string(request.PacienteData.Rg.Where(char.IsLetterOrDigit).ToArray()) : null;

                if (string.IsNullOrWhiteSpace(cpfLimpo) || !Utils.Utils.ValidarCpf(cpfLimpo))
                    throw new CustomException("Informe um CPF válido.", 400);

                var cpfExistente = await context.Pacientes.AnyAsync(p => p.Cpf == cpfLimpo);
                if (cpfExistente)
                    throw new CustomException("Já existe um usuário cadastrado com o mesmo CPF.", 409);

                if (!string.IsNullOrWhiteSpace(rgLimpo))
                {
                    var rgExistente = await context.Pacientes.AnyAsync(p => p.Rg == rgLimpo);
                    if (rgExistente)
                        throw new CustomException("Já existe um usuário cadastrado com o mesmo RG.", 409);
                }

                var novoPaciente = new Paciente
                {
                    IdUsuario = novoUsuario.IdUsuario,
                    NomeCompleto = request.PacienteData.NomeCompleto!,
                    DataNascimento = request.PacienteData.DataNascimento,
                    Cpf = cpfLimpo!,
                    Telefone = request.PacienteData.Telefone ?? string.Empty,
                    Endereco = request.PacienteData.Endereco ?? string.Empty,
                    HistoricoClinico = request.PacienteData.HistoricoClinico ?? string.Empty,
                    Rg = rgLimpo!,
                    Sexo = request.PacienteData.Sexo!,
                    Convenio = request.PacienteData.Convenio ?? string.Empty,
                    Email = request.PacienteData.EmailPaciente
                };
                context.Pacientes.Add(novoPaciente);
                await context.SaveChangesAsync();
            }
            else if (request.Perfil == "PROFISSIONAL")
            {
                if (request.ProfissionalData == null)
                    throw new CustomException("Informe os dados do profissional.");

                // Sanitizações
                var cpfLimpo = request.ProfissionalData.Cpf != null ? new string(request.ProfissionalData.Cpf.Where(char.IsDigit).ToArray()) : null;
                var rgLimpo = request.ProfissionalData.Rg != null ? new string(request.ProfissionalData.Rg.Where(char.IsLetterOrDigit).ToArray()) : null;

                // Validar CPF
                if (string.IsNullOrWhiteSpace(cpfLimpo) || !Utils.Utils.ValidarCpf(cpfLimpo))
                    throw new CustomException("Informe um CPF válido.");

                // Verificar se o CPF já está cadastrado entre profissionais
                var cpfProfissionalExistente = await context.Profissionais.AnyAsync(ps => ps.Cpf == cpfLimpo);
                if (cpfProfissionalExistente)
                    throw new CustomException("Já existe um usuário cadastrado com o mesmo CPF.", 409);

                // Verificar se o RG já está cadastrado entre profissionais
                if (!string.IsNullOrWhiteSpace(rgLimpo))
                {
                    var rgProfissionalExistente = await context.Profissionais.AnyAsync(ps => ps.Rg == rgLimpo);
                    if (rgProfissionalExistente)
                        throw new CustomException("Já existe um usuário cadastrado com o mesmo RG.", 409);
                }

                // Verificar se o CRM/Conselho já está cadastrado
                var crmExistente = await context.Profissionais.AnyAsync(ps => ps.CrmOuConselho == request.ProfissionalData.CrmOuConselho);
                if (crmExistente)
                    throw new CustomException("Já existe um usuário cadastrado com o mesmo CRM/Conselho.", 409);

                var novoProfissional = new Profissional
                {
                    IdUsuario = novoUsuario.IdUsuario,
                    NomeCompleto = request.ProfissionalData.NomeCompleto,
                    Cpf = cpfLimpo!,
                    Rg = rgLimpo!,
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
