using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddRelacaoProfissionalPaciente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RelacoesProfissionalPaciente",
                columns: table => new
                {
                    IdRelacao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProfissional = table.Column<int>(type: "int", nullable: false),
                    IdPaciente = table.Column<int>(type: "int", nullable: false),
                    StatusRelacao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelacoesProfissionalPaciente", x => x.IdRelacao);
                    table.ForeignKey(
                        name: "FK_RelacoesProfissionalPaciente_Pacientes_IdPaciente",
                        column: x => x.IdPaciente,
                        principalTable: "Pacientes",
                        principalColumn: "IdPaciente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RelacoesProfissionalPaciente_Profissionais_IdProfissional",
                        column: x => x.IdProfissional,
                        principalTable: "Profissionais",
                        principalColumn: "IdProfissional",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelacoesProfissionalPaciente_IdPaciente",
                table: "RelacoesProfissionalPaciente",
                column: "IdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_RelacoesProfissionalPaciente_IdProfissional_IdPaciente",
                table: "RelacoesProfissionalPaciente",
                columns: new[] { "IdProfissional", "IdPaciente" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RelacoesProfissionalPaciente");
        }
    }
}
