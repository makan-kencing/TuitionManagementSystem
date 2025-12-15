using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TuitionManagementSystem.Web.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExplicitForeignKeyColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_AttendanceCodes_CodeId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Families_Parent_FamilyId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Families_Student_FamilyId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Parent_FamilyId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Student_FamilyId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_StudentId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionFiles_SubmissionId",
                table: "SubmissionFiles");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_CourseId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_FamilyInvites_FamilyId",
                table: "FamilyInvites");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_CourseTeachers_CourseId",
                table: "CourseTeachers");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_SessionId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_AnnouncementFiles_AnnouncementId",
                table: "AnnouncementFiles");

            migrationBuilder.DropColumn(
                name: "Parent_FamilyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Student_FamilyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "CodeId",
                table: "Sessions",
                newName: "AttendanceCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_CodeId",
                table: "Sessions",
                newName: "IX_Sessions_AttendanceCodeId");

            migrationBuilder.AlterColumn<string>(
                name: "Last4",
                table: "PaymentMethods",
                type: "character varying(4)",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Brand",
                table: "PaymentMethods",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bank",
                table: "PaymentMethods",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Generic",
                table: "PaymentMethods",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Invoices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FamilyMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FamilyId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    JoinedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FamilyMembers_Families_FamilyId",
                        column: x => x.FamilyId,
                        principalTable: "Families",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FamilyMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_StudentId_AssignmentId",
                table: "Submissions",
                columns: new[] { "StudentId", "AssignmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionFiles_SubmissionId_FileId",
                table: "SubmissionFiles",
                columns: new[] { "SubmissionId", "FileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CourseId_ClassroomId",
                table: "Sessions",
                columns: new[] { "CourseId", "ClassroomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FamilyInvites_FamilyId_UserId",
                table: "FamilyInvites",
                columns: new[] { "FamilyId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId_CourseId",
                table: "Enrollments",
                columns: new[] { "StudentId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachers_CourseId_TeacherId",
                table: "CourseTeachers",
                columns: new[] { "CourseId", "TeacherId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_SessionId_StudentId",
                table: "Attendances",
                columns: new[] { "SessionId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementFiles_AnnouncementId_FileId",
                table: "AnnouncementFiles",
                columns: new[] { "AnnouncementId", "FileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FamilyMembers_FamilyId_UserId",
                table: "FamilyMembers",
                columns: new[] { "FamilyId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FamilyMembers_UserId",
                table: "FamilyMembers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_AttendanceCodes_AttendanceCodeId",
                table: "Sessions",
                column: "AttendanceCodeId",
                principalTable: "AttendanceCodes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_AttendanceCodes_AttendanceCodeId",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "FamilyMembers");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_StudentId_AssignmentId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionFiles_SubmissionId_FileId",
                table: "SubmissionFiles");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_CourseId_ClassroomId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_FamilyInvites_FamilyId_UserId",
                table: "FamilyInvites");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId_CourseId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_CourseTeachers_CourseId_TeacherId",
                table: "CourseTeachers");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_SessionId_StudentId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_AnnouncementFiles_AnnouncementId_FileId",
                table: "AnnouncementFiles");

            migrationBuilder.DropColumn(
                name: "Generic",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "AttendanceCodeId",
                table: "Sessions",
                newName: "CodeId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_AttendanceCodeId",
                table: "Sessions",
                newName: "IX_Sessions_CodeId");

            migrationBuilder.AddColumn<int>(
                name: "Parent_FamilyId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Student_FamilyId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Last4",
                table: "PaymentMethods",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(4)",
                oldMaxLength: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Brand",
                table: "PaymentMethods",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bank",
                table: "PaymentMethods",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PaymentMethods",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Invoices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Parent_FamilyId",
                table: "Users",
                column: "Parent_FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Student_FamilyId",
                table: "Users",
                column: "Student_FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_StudentId",
                table: "Submissions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionFiles_SubmissionId",
                table: "SubmissionFiles",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CourseId",
                table: "Sessions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyInvites_FamilyId",
                table: "FamilyInvites",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachers_CourseId",
                table: "CourseTeachers",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_SessionId",
                table: "Attendances",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementFiles_AnnouncementId",
                table: "AnnouncementFiles",
                column: "AnnouncementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_AttendanceCodes_CodeId",
                table: "Sessions",
                column: "CodeId",
                principalTable: "AttendanceCodes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Families_Parent_FamilyId",
                table: "Users",
                column: "Parent_FamilyId",
                principalTable: "Families",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Families_Student_FamilyId",
                table: "Users",
                column: "Student_FamilyId",
                principalTable: "Families",
                principalColumn: "Id");
        }
    }
}
