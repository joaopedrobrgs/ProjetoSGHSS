using Microsoft.EntityFrameworkCore;
using SGHSS_Backend.Data.Entities;

namespace SGHSS_Backend.Data;

public class SGHSSDbContext : DbContext
{
    public SGHSSDbContext(DbContextOptions<SGHSSDbContext> options) : base(options)
    {
    }

    // DbSets representam as tabelas do seu banco de dados
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<Profissional> Profissionais { get; set; }
    public DbSet<Consulta> Consultas { get; set; }

    // Método para configurar o mapeamento das entidades para as tabelas do banco de dados
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mapeamento explícito para as chaves primárias e nomes de tabelas, se necessário
        // Para 'id_usuario' como PK e auto-incremento
        modelBuilder.Entity<Usuario>().HasKey(u => u.IdUsuario);
        modelBuilder.Entity<Usuario>().Property(u => u.IdUsuario).ValueGeneratedOnAdd();
        modelBuilder.Entity<Usuario>().ToTable("Usuarios"); // Mapeia para a tabela "Usuarios"

        modelBuilder.Entity<Paciente>().HasKey(p => p.IdPaciente);
        modelBuilder.Entity<Paciente>().Property(p => p.IdPaciente).ValueGeneratedOnAdd();
        modelBuilder.Entity<Paciente>().ToTable("Pacientes");

        modelBuilder.Entity<Profissional>().HasKey(ps => ps.IdProfissional);
        modelBuilder.Entity<Profissional>().Property(ps => ps.IdProfissional).ValueGeneratedOnAdd();
        modelBuilder.Entity<Profissional>().ToTable("Profissionais");

        modelBuilder.Entity<Consulta>().HasKey(c => c.IdConsulta);
        modelBuilder.Entity<Consulta>().Property(c => c.IdConsulta).ValueGeneratedOnAdd();
        modelBuilder.Entity<Consulta>().ToTable("Consultas");


        // Configuração dos relacionamentos (Fluent API)
        // Relacionamento Usuario-Paciente (1:0 ou 1:1)
        modelBuilder.Entity<Paciente>()
            .HasOne(p => p.Usuario)
            .WithOne(u => u.Paciente)
            .HasForeignKey<Paciente>(p => p.IdUsuario)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade); // Se um usuário for deletado, o paciente associado também é

        // Relacionamento Usuario-Profissional (1:0 ou 1:1)
        modelBuilder.Entity<Profissional>()
            .HasOne(ps => ps.Usuario)
            .WithOne(u => u.Profissional)
            .HasForeignKey<Profissional>(ps => ps.IdUsuario)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade); // Se um usuário for deletado, o profissional associado também é

        // Relacionamento Paciente-Consulta (1:N)
        modelBuilder.Entity<Consulta>()
            .HasOne(c => c.Paciente)
            .WithMany(p => p.Consultas)
            .HasForeignKey(c => c.IdPaciente)
            .OnDelete(DeleteBehavior.Restrict); // Não deleta o paciente se ele tiver consultas

        // Relacionamento Profissional-Consulta (1:N)
        modelBuilder.Entity<Consulta>()
            .HasOne(c => c.Profissional)
            .WithMany(ps => ps.Consultas)
            .HasForeignKey(c => c.IdProfissional)
            .OnDelete(DeleteBehavior.Restrict); // Não deleta o profissional se ele tiver consultas


        // Configurações adicionais para mapeamento de nomes de coluna se forem diferentes das convenções
        // Exemplo: se na tabela Usuarios a coluna fosse 'user_email' e na classe é 'Email'
        // modelBuilder.Entity<Usuario>().Property(u => u.Email).HasColumnName("user_email");

        // Configurações para UNIQUE constraints não inferidas
        modelBuilder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Paciente>().HasIndex(p => p.Cpf).IsUnique();
        modelBuilder.Entity<Profissional>().HasIndex(ps => ps.CrmOuConselho).IsUnique();


        base.OnModelCreating(modelBuilder);
    }
}
