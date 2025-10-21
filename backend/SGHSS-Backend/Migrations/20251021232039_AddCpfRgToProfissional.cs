using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCpfRgToProfissional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "Profissionais",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Rg",
                table: "Profissionais",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "Profissionais");

            migrationBuilder.DropColumn(
                name: "Rg",
                table: "Profissionais");
        }
    }
}
