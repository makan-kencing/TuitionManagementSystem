using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuitionManagementSystem.Web.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReverseSessionAndAttendanceCodeRelationDirection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_AttendanceCodes_AttendanceCodeId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_AttendanceCodeId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "AttendanceCodeId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "CodeGeneratedAt",
                table: "Sessions");

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "AttendanceCodes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCodes_SessionId",
                table: "AttendanceCodes",
                column: "SessionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceCodes_Sessions_SessionId",
                table: "AttendanceCodes",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceCodes_Sessions_SessionId",
                table: "AttendanceCodes");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceCodes_SessionId",
                table: "AttendanceCodes");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "AttendanceCodes");

            migrationBuilder.AddColumn<int>(
                name: "AttendanceCodeId",
                table: "Sessions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CodeGeneratedAt",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_AttendanceCodeId",
                table: "Sessions",
                column: "AttendanceCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_AttendanceCodes_AttendanceCodeId",
                table: "Sessions",
                column: "AttendanceCodeId",
                principalTable: "AttendanceCodes",
                principalColumn: "Id");
        }
    }
}
