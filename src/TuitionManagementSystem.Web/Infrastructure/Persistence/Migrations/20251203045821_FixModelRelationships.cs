using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TuitionManagementSystem.Web.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixModelRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_User_UserId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Courses_CourseId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Schedules_ScheduleId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Announcements_AnnouncementId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Submissions_SubmissionId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleRecurrencePatterns_Schedules_ScheduleId",
                table: "ScheduleRecurrencePatterns");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Classrooms_ClassroomId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Courses_CourseId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_CourseId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Payments_MethodId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Files_AnnouncementId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_SubmissionId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Courses_ScheduleId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "AnnouncementId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "SubmissionId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Accounts");

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ClassroomId",
                table: "Sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Schedules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ScheduleId",
                table: "ScheduleRecurrencePatterns",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueAt",
                table: "Invoices",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Announcements",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "AnnouncementFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnnouncementId = table.Column<int>(type: "integer", nullable: false),
                    FileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnouncementFiles_Announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnnouncementFiles_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseTeachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTeachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseTeachers_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseTeachers_User_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubmissionId = table.Column<int>(type: "integer", nullable: false),
                    FileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionFiles_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmissionFiles_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_AccountId",
                table: "User",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_CourseId",
                table: "Schedules",
                column: "CourseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_MethodId",
                table: "Payments",
                column: "MethodId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementFiles_AnnouncementId",
                table: "AnnouncementFiles",
                column: "AnnouncementId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementFiles_FileId",
                table: "AnnouncementFiles",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachers_CourseId",
                table: "CourseTeachers",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachers_TeacherId",
                table: "CourseTeachers",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionFiles_FileId",
                table: "SubmissionFiles",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionFiles_SubmissionId",
                table: "SubmissionFiles",
                column: "SubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Courses_CourseId",
                table: "Announcements",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleRecurrencePatterns_Schedules_ScheduleId",
                table: "ScheduleRecurrencePatterns",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Courses_CourseId",
                table: "Schedules",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Classrooms_ClassroomId",
                table: "Sessions",
                column: "ClassroomId",
                principalTable: "Classrooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Accounts_AccountId",
                table: "User",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Courses_CourseId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleRecurrencePatterns_Schedules_ScheduleId",
                table: "ScheduleRecurrencePatterns");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Courses_CourseId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Classrooms_ClassroomId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Accounts_AccountId",
                table: "User");

            migrationBuilder.DropTable(
                name: "AnnouncementFiles");

            migrationBuilder.DropTable(
                name: "CourseTeachers");

            migrationBuilder.DropTable(
                name: "SubmissionFiles");

            migrationBuilder.DropIndex(
                name: "IX_User_AccountId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_CourseId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Payments_MethodId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Schedules");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "User",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClassroomId",
                table: "Sessions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ScheduleId",
                table: "ScheduleRecurrencePatterns",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueAt",
                table: "Invoices",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AnnouncementId",
                table: "Files",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubmissionId",
                table: "Files",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Courses",
                type: "integer",
                nullable: true);

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
                name: "UserId",
                table: "Accounts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_CourseId",
                table: "User",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_MethodId",
                table: "Payments",
                column: "MethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_AnnouncementId",
                table: "Files",
                column: "AnnouncementId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_SubmissionId",
                table: "Files",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ScheduleId",
                table: "Courses",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_User_UserId",
                table: "Accounts",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Courses_CourseId",
                table: "Announcements",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Schedules_ScheduleId",
                table: "Courses",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Announcements_AnnouncementId",
                table: "Files",
                column: "AnnouncementId",
                principalTable: "Announcements",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Submissions_SubmissionId",
                table: "Files",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleRecurrencePatterns_Schedules_ScheduleId",
                table: "ScheduleRecurrencePatterns",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Classrooms_ClassroomId",
                table: "Sessions",
                column: "ClassroomId",
                principalTable: "Classrooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Courses_CourseId",
                table: "User",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }
    }
}
