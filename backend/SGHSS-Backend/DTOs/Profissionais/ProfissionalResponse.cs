using System;

namespace SGHSS_Backend.DTOs.Profissionais;

public class ProfissionalResponse
{
    public int IdProfissional { get; set; }
    public int IdUsuario { get; set; }
    public string NomeCompleto { get; set; }
    public string CrmOuConselho { get; set; }
    public string Especialidade { get; set; }
    public string Telefone { get; set; }
    public string EmailProfissional { get; set; }
    public string DisponibilidadeAgenda { get; set; }
    public string EmailUsuario { get; set; }
}
