using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TuitionManagementSystem.Web.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFamilyInviteModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Family_FamilyId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Family_Parent_FamilyId",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Family",
                table: "Family");

            migrationBuilder.RenameTable(
                name: "Family",
                newName: "Families");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Notifications",
                type: "character varying(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FamilyInviteId",
                table: "Notifications",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Families",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Families",
                table: "Families",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FamilyInvites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FamilyId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RequesterId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FamilyInvites_Families_FamilyId",
                        column: x => x.FamilyId,
                        principalTable: "Families",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FamilyInvites_User_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FamilyInvites_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_FamilyInviteId",
                table: "Notifications",
                column: "FamilyInviteId");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyInvites_FamilyId",
                table: "FamilyInvites",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyInvites_RequesterId",
                table: "FamilyInvites",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyInvites_UserId",
                table: "FamilyInvites",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_FamilyInvites_FamilyInviteId",
                table: "Notifications",
                column: "FamilyInviteId",
                principalTable: "FamilyInvites",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_FamilyInvites_FamilyInviteId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Families_FamilyId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Families_Parent_FamilyId",
                table: "User");

            migrationBuilder.DropTable(
                name: "FamilyInvites");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_FamilyInviteId",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Families",
                table: "Families");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "FamilyInviteId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Families");

            migrationBuilder.RenameTable(
                name: "Families",
                newName: "Family");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Family",
                table: "Family",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Family_FamilyId",
                table: "User",
                column: "FamilyId",
                principalTable: "Family",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Family_Parent_FamilyId",
                table: "User",
                column: "Parent_FamilyId",
                principalTable: "Family",
                principalColumn: "Id");
        }
    }
}
