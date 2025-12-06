using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuitionManagementSystem.Web.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixDuplicatedShadowColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Courses_CourseId1",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Courses_CourseId2",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Courses_CourseId3",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_CourseId1",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_CourseId2",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_CourseId3",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "CourseId2",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "CourseId3",
                table: "Announcements");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId1",
                table: "Announcements",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourseId2",
                table: "Announcements",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourseId3",
                table: "Announcements",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_CourseId1",
                table: "Announcements",
                column: "CourseId1");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_CourseId2",
                table: "Announcements",
                column: "CourseId2");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_CourseId3",
                table: "Announcements",
                column: "CourseId3");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Courses_CourseId1",
                table: "Announcements",
                column: "CourseId1",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Courses_CourseId2",
                table: "Announcements",
                column: "CourseId2",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Courses_CourseId3",
                table: "Announcements",
                column: "CourseId3",
                principalTable: "Courses",
                principalColumn: "Id");
        }
    }
}
