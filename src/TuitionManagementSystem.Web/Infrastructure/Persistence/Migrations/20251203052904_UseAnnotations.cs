using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuitionManagementSystem.Web.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UseAnnotations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Courses_CourseId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Courses_Material_CourseId1",
                table: "Announcements");

            migrationBuilder.RenameColumn(
                name: "Material_CourseId1",
                table: "Announcements",
                newName: "CourseId3");

            migrationBuilder.RenameIndex(
                name: "IX_Announcements_Material_CourseId1",
                table: "Announcements",
                newName: "IX_Announcements_CourseId3");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Announcements",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourseId2",
                table: "Announcements",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_CourseId2",
                table: "Announcements",
                column: "CourseId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Courses_CourseId",
                table: "Announcements",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Courses_CourseId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Courses_CourseId2",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Courses_CourseId3",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_CourseId2",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "CourseId2",
                table: "Announcements");

            migrationBuilder.RenameColumn(
                name: "CourseId3",
                table: "Announcements",
                newName: "Material_CourseId1");

            migrationBuilder.RenameIndex(
                name: "IX_Announcements_CourseId3",
                table: "Announcements",
                newName: "IX_Announcements_Material_CourseId1");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Announcements",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Courses_CourseId",
                table: "Announcements",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Courses_Material_CourseId1",
                table: "Announcements",
                column: "Material_CourseId1",
                principalTable: "Courses",
                principalColumn: "Id");
        }
    }
}
