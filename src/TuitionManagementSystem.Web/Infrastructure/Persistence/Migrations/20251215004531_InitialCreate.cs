#nullable disable

namespace TuitionManagementSystem.Web.Infrastructure.Persistence.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLocal",
                table: "Files");

            migrationBuilder.AddColumn<string>(
                name: "CanonicalPath",
                table: "Files",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanonicalPath",
                table: "Files");

            migrationBuilder.AddColumn<bool>(
                name: "IsLocal",
                table: "Files",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
