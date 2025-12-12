using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuitionManagementSystem.Web.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExplicitUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_User_CreatedById",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_User_StudentId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTeachers_User_TeacherId",
                table: "CourseTeachers");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_User_StudentId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyInvites_User_RequesterId",
                table: "FamilyInvites");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyInvites_User_UserId",
                table: "FamilyInvites");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_User_CreatedById",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_User_StudentId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_User_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_User_StudentId",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Accounts_AccountId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Families_FamilyId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Families_Parent_FamilyId",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_User_Parent_FamilyId",
                table: "Users",
                newName: "IX_Users_Parent_FamilyId");

            migrationBuilder.RenameIndex(
                name: "IX_User_FamilyId",
                table: "Users",
                newName: "IX_Users_FamilyId");

            migrationBuilder.RenameIndex(
                name: "IX_User_AccountId",
                table: "Users",
                newName: "IX_Users_AccountId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishedAt",
                table: "Announcements",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Users_CreatedById",
                table: "Announcements",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Users_StudentId",
                table: "Attendances",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTeachers_Users_TeacherId",
                table: "CourseTeachers",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Users_StudentId",
                table: "Enrollments",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyInvites_Users_RequesterId",
                table: "FamilyInvites",
                column: "RequesterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyInvites_Users_UserId",
                table: "FamilyInvites",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Users_CreatedById",
                table: "Files",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Users_StudentId",
                table: "Invoices",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Users_StudentId",
                table: "Submissions",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Accounts_AccountId",
                table: "Users",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Families_FamilyId",
                table: "Users",
                column: "FamilyId",
                principalTable: "Families",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Families_Parent_FamilyId",
                table: "Users",
                column: "Parent_FamilyId",
                principalTable: "Families",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Users_CreatedById",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Users_StudentId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTeachers_Users_TeacherId",
                table: "CourseTeachers");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Users_StudentId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyInvites_Users_RequesterId",
                table: "FamilyInvites");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyInvites_Users_UserId",
                table: "FamilyInvites");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Users_CreatedById",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Users_StudentId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Users_StudentId",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Accounts_AccountId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Families_FamilyId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Families_Parent_FamilyId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Parent_FamilyId",
                table: "User",
                newName: "IX_User_Parent_FamilyId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_FamilyId",
                table: "User",
                newName: "IX_User_FamilyId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_AccountId",
                table: "User",
                newName: "IX_User_AccountId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishedAt",
                table: "Announcements",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_User_CreatedById",
                table: "Announcements",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_User_StudentId",
                table: "Attendances",
                column: "StudentId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTeachers_User_TeacherId",
                table: "CourseTeachers",
                column: "TeacherId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_User_StudentId",
                table: "Enrollments",
                column: "StudentId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyInvites_User_RequesterId",
                table: "FamilyInvites",
                column: "RequesterId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyInvites_User_UserId",
                table: "FamilyInvites",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_User_CreatedById",
                table: "Files",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_User_StudentId",
                table: "Invoices",
                column: "StudentId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_User_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_User_StudentId",
                table: "Submissions",
                column: "StudentId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Accounts_AccountId",
                table: "User",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Families_FamilyId",
                table: "User",
                column: "FamilyId",
                principalTable: "Families",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Families_Parent_FamilyId",
                table: "User",
                column: "Parent_FamilyId",
                principalTable: "Families",
                principalColumn: "Id");
        }
    }
}
