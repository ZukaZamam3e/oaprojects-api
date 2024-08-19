using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.OAIdentity.Migrations
{
    public partial class _0003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EXPIRY_DATE_UTC",
                table: "OA_USER_TOKEN",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ISSUED_AT",
                table: "OA_USER_TOKEN",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ISSUED_AT_DATE_UTC",
                table: "OA_USER_TOKEN",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EXPIRY_DATE_UTC",
                table: "OA_USER_TOKEN");

            migrationBuilder.DropColumn(
                name: "ISSUED_AT",
                table: "OA_USER_TOKEN");

            migrationBuilder.DropColumn(
                name: "ISSUED_AT_DATE_UTC",
                table: "OA_USER_TOKEN");
        }
    }
}
