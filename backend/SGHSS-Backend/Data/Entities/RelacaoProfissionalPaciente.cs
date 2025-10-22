using System;

namespace SGHSS_Backend.Data.Entities;

public class RelacaoProfissionalPaciente
{
    public int IdRelacao { get; set; }
    public int IdProfissional { get; set; }
    public int IdPaciente { get; set; }
    public string StatusRelacao { get; set; } // "Ativo" | "Inativo"

    // Navegação
    public virtual Profissional Profissional { get; set; }
    public virtual Paciente Paciente { get; set; }
}
