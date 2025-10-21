using System;

namespace SGHSS_Backend.DTOs.Profissionais;

public class ProfissionalResponse
{
    public int IdProfissional { get; set; }
    public int IdUsuario { get; set; }
    public required string NomeCompleto { get; set; }
    public required string Cpf { get; set; }
    public required string Rg { get; set; }
    public required string CrmOuConselho { get; set; }
    public required string Especialidade { get; set; }
    public required string Telefone { get; set; }
    public required string EmailProfissional { get; set; }
    public required string DisponibilidadeAgenda { get; set; }
    public required string EmailUsuario { get; set; }
}
