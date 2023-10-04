using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.OAIdentity.Migrations
{
    public partial class _0001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OA_USER",
                columns: table => new
                {
                    USER_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USER_GUID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    USER_NAME = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    USER_LOGIN_TYPE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DATE_ADDED = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OA_USER", x => x.USER_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OA_USER");
        }
    }
}
