using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.OAIdentity.Migrations
{
    public partial class _0002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EXPIRATION_TIME",
                table: "OA_USER_TOKEN");

            migrationBuilder.AddColumn<int>(
                name: "EXPIRY_TIME",
                table: "OA_USER_TOKEN",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EXPIRY_TIME",
                table: "OA_USER_TOKEN");

            migrationBuilder.AddColumn<DateTime>(
                name: "EXPIRATION_TIME",
                table: "OA_USER_TOKEN",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
