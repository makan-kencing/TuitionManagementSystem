using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuitionManagementSystem.Web.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEnrollmentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnrollmentId",
                table: "Invoices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_EnrollmentId",
                table: "Invoices",
                column: "EnrollmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Enrollments_EnrollmentId",
                table: "Invoices",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Enrollments_EnrollmentId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_EnrollmentId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "EnrollmentId",
                table: "Invoices");
        }
    }
}
