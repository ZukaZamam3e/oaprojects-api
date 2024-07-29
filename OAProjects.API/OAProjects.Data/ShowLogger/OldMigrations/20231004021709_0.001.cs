using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.ShowLogger.Migrations
{
    public partial class _0001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SL_CODE_VALUE",
                columns: table => new
                {
                    CODE_VALUE_ID = table.Column<int>(type: "int", nullable: false),
                    CODE_TABLE_ID = table.Column<int>(type: "int", nullable: false),
                    DECODE_TXT = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EXTRA_INFO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_CODE_VALUE", x => x.CODE_VALUE_ID);
                });

            migrationBuilder.CreateTable(
                name: "SL_SHOW",
                columns: table => new
                {
                    SHOW_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USER_ID = table.Column<int>(type: "int", nullable: false),
                    SHOW_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SHOW_TYPE_ID = table.Column<int>(type: "int", nullable: false),
                    SEASON_NUMBER = table.Column<int>(type: "int", nullable: true),
                    EPISODE_NUMBER = table.Column<int>(type: "int", nullable: true),
                    DATE_WATCHED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SHOW_NOTES = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_SHOW", x => x.SHOW_ID);
                });

            migrationBuilder.CreateTable(
                name: "SL_USER_PREF",
                columns: table => new
                {
                    USER_PREF_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USER_ID = table.Column<int>(type: "int", nullable: false),
                    DEFAULT_AREA = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_USER_PREF", x => x.USER_PREF_ID);
                });

            migrationBuilder.InsertData(
                table: "SL_CODE_VALUE",
                columns: new[] { "CODE_VALUE_ID", "CODE_TABLE_ID", "DECODE_TXT", "EXTRA_INFO" },
                values: new object[,]
                {
                    { 1000, 1, "TV", null },
                    { 1001, 1, "Movie", null },
                    { 1002, 1, "AMC", null },
                    { 2000, 2, "A-list Ticket", null },
                    { 2001, 2, "Ticket", null },
                    { 2002, 2, "Purchase", null },
                    { 2003, 2, "AMC A-list", null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SL_CODE_VALUE");

            migrationBuilder.DropTable(
                name: "SL_SHOW");

            migrationBuilder.DropTable(
                name: "SL_USER_PREF");
        }
    }
}
