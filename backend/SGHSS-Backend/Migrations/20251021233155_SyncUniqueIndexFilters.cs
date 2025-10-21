using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class SyncUniqueIndexFilters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Profissionais_Cpf",
                table: "Profissionais");

            migrationBuilder.DropIndex(
                name: "IX_Profissionais_Rg",
                table: "Profissionais");

            migrationBuilder.DropIndex(
                name: "IX_Pacientes_Rg",
                table: "Pacientes");

            migrationBuilder.CreateIndex(
                name: "IX_Profissionais_Cpf",
                table: "Profissionais",
                column: "Cpf",
                unique: true,
                filter: "[Cpf] IS NOT NULL AND [Cpf] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Profissionais_Rg",
                table: "Profissionais",
                column: "Rg",
                unique: true,
                filter: "[Rg] IS NOT NULL AND [Rg] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_Rg",
                table: "Pacientes",
                column: "Rg",
                unique: true,
                filter: "[Rg] IS NOT NULL AND [Rg] <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Profissionais_Cpf",
                table: "Profissionais");

            migrationBuilder.DropIndex(
                name: "IX_Profissionais_Rg",
                table: "Profissionais");

            migrationBuilder.DropIndex(
                name: "IX_Pacientes_Rg",
                table: "Pacientes");

            migrationBuilder.CreateIndex(
                name: "IX_Profissionais_Cpf",
                table: "Profissionais",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profissionais_Rg",
                table: "Profissionais",
                column: "Rg",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_Rg",
                table: "Pacientes",
                column: "Rg",
                unique: true);
        }
    }
}
