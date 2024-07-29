using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.ShowLogger.Migrations
{
    public partial class _0002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SL_FRIEND",
                columns: table => new
                {
                    FRIEND_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USER_ID = table.Column<int>(type: "int", nullable: false),
                    FRIEND_USER_ID = table.Column<int>(type: "int", nullable: false),
                    CREATED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_FRIEND", x => x.FRIEND_ID);
                });

            migrationBuilder.CreateTable(
                name: "SL_FRIEND_REQUEST",
                columns: table => new
                {
                    FRIEND_REQUEST_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SENT_USER_ID = table.Column<int>(type: "int", nullable: false),
                    RECEIVED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    DATE_SENT = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_FRIEND_REQUEST", x => x.FRIEND_REQUEST_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SL_FRIEND");

            migrationBuilder.DropTable(
                name: "SL_FRIEND_REQUEST");
        }
    }
}
