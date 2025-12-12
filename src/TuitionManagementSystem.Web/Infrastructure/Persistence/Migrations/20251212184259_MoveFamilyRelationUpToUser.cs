using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuitionManagementSystem.Web.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveFamilyRelationUpToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Student_FamilyId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Student_FamilyId",
                table: "Users",
                column: "Student_FamilyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Families_Student_FamilyId",
                table: "Users",
                column: "Student_FamilyId",
                principalTable: "Families",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Families_Student_FamilyId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Student_FamilyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Student_FamilyId",
                table: "Users");
        }
    }
}
