using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToPacientes_AndUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Rg",
                table: "Profissionais",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Profissionais",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Rg",
                table: "Pacientes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Pacientes",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true);

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

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Pacientes");

            migrationBuilder.AlterColumn<string>(
                name: "Rg",
                table: "Profissionais",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Profissionais",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Rg",
                table: "Pacientes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
