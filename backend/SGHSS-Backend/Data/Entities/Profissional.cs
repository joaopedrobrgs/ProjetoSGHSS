using System;
using System.Collections.Generic;

namespace SGHSS_Backend.Data.Entities;

public class Profissional
{
    public int IdProfissional { get; set; } // Corresponde a id_profissional no banco
    public int IdUsuario { get; set; } // Chave Estrangeira para Usuario
    public string NomeCompleto { get; set; }
    public string CrmOuConselho { get; set; }
    public string Especialidade { get; set; }
    public string Telefone { get; set; }
    public string EmailProfissional { get; set; }
    public string DisponibilidadeAgenda { get; set; }

    // Propriedades de navegação
    public virtual Usuario Usuario { get; set; } // Relacionamento 1:1 ou 1:0 com Usuario
    public virtual ICollection<Consulta> Consultas { get; set; } // Um profissional pode ter muitas consultas
}
