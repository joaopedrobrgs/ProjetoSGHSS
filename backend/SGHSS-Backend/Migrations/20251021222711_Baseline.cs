using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Baseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Baseline migration: intentionally empty. The existing database schema is treated as up-to-date.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Baseline migration: no-op on downgrade as well.
        }
    }
}
