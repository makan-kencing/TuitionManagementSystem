using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuitionManagementSystem.Web.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixComputedDateTimeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountSessions_SessionId",
                table: "AccountSessions");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionId",
                table: "AccountSessions",
                type: "uuid using \"SessionId\"::uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSessions_SessionId",
                table: "AccountSessions",
                column: "SessionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountSessions_SessionId",
                table: "AccountSessions");

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "AccountSessions",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSessions_SessionId",
                table: "AccountSessions",
                column: "SessionId");
        }
    }
}
