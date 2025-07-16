using System;
using System.Collections.Generic; // Necessário para coleções de navegação

namespace SGHSS_Backend.Data.Entities;

public class Usuario
{
    public int IdUsuario { get; set; } // Corresponde a id_usuario no banco
    public string Email { get; set; }
    public string Senha { get; set; } // Armazenar hash da senha, não a senha em texto claro!
    public string Perfil { get; set; }
    public DateTime DataCriacao { get; set; }
    public bool Ativo { get; set; }

    // Propriedades de navegação (opcional, mas boa prática para EF Core)
    public virtual Paciente? Paciente { get; set; } // Um usuário pode ser um paciente
    public virtual Profissional? Profissional { get; set; } // Um usuário pode ser um profissional
}
