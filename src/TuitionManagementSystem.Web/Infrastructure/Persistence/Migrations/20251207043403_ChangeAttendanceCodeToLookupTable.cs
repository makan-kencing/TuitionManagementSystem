using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TuitionManagementSystem.Web.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAttendanceCodeToLookupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceCodes_Sessions_SessionId",
                table: "AttendanceCodes");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceCodes_DatedFor_Code",
                table: "AttendanceCodes");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceCodes_SessionId",
                table: "AttendanceCodes");

            migrationBuilder.DropColumn(
                name: "DatedFor",
                table: "AttendanceCodes");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "AttendanceCodes");

            migrationBuilder.AddColumn<DateTime>(
                name: "CodeGeneratedAt",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CodeId",
                table: "Sessions",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AttendanceCodes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CodeId",
                table: "Sessions",
                column: "CodeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCodes_Code",
                table: "AttendanceCodes",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_AttendanceCodes_CodeId",
                table: "Sessions",
                column: "CodeId",
                principalTable: "AttendanceCodes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_AttendanceCodes_CodeId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_CodeId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceCodes_Code",
                table: "AttendanceCodes");

            migrationBuilder.DropColumn(
                name: "CodeGeneratedAt",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "CodeId",
                table: "Sessions");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AttendanceCodes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DatedFor",
                table: "AttendanceCodes",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "AttendanceCodes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCodes_DatedFor_Code",
                table: "AttendanceCodes",
                columns: new[] { "DatedFor", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCodes_SessionId",
                table: "AttendanceCodes",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceCodes_Sessions_SessionId",
                table: "AttendanceCodes",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
