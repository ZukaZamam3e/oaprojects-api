using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.ShowLogger.Migrations
{
    public partial class _0004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "INFO_ID",
                table: "SL_SHOW",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RESTART_BINGE",
                table: "SL_SHOW",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SL_MOVIE_INFO",
                columns: table => new
                {
                    MOVIE_INFO_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MOVIE_NAME = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MOVIE_OVERVIEW = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    API_TYPE = table.Column<int>(type: "int", nullable: true),
                    API_ID = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    OTHER_NAMES = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RUNTIME = table.Column<int>(type: "int", nullable: true),
                    AIR_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LAST_DATA_REFRESH = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LAST_UPDATED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IMAGE_URL = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_MOVIE_INFO", x => x.MOVIE_INFO_ID);
                });

            migrationBuilder.CreateTable(
                name: "SL_TV_INFO",
                columns: table => new
                {
                    TV_INFO_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SHOW_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SHOW_OVERVIEW = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    API_TYPE = table.Column<int>(type: "int", nullable: true),
                    API_ID = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    OTHER_NAMES = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    LAST_DATA_REFRESH = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LAST_UPDATED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IMAGE_URL = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_TV_INFO", x => x.TV_INFO_ID);
                });

            migrationBuilder.CreateTable(
                name: "SL_TV_EPISODE_INFO",
                columns: table => new
                {
                    TV_EPISODE_INFO_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TV_INFO_ID = table.Column<int>(type: "int", nullable: false),
                    API_TYPE = table.Column<int>(type: "int", nullable: true),
                    API_ID = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    SEASON_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EPISODE_NAME = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SEASON_NUMBER = table.Column<int>(type: "int", nullable: true),
                    EPISODE_NUMBER = table.Column<int>(type: "int", nullable: true),
                    EPISODE_OVERVIEW = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RUNTIME = table.Column<int>(type: "int", nullable: true),
                    AIR_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IMAGE_URL = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_TV_EPISODE_INFO", x => x.TV_EPISODE_INFO_ID);
                    table.ForeignKey(
                        name: "FK_SL_TV_EPISODE_INFO_SL_TV_INFO_TV_INFO_ID",
                        column: x => x.TV_INFO_ID,
                        principalTable: "SL_TV_INFO",
                        principalColumn: "TV_INFO_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SL_TV_EPISODE_INFO_TV_INFO_ID",
                table: "SL_TV_EPISODE_INFO",
                column: "TV_INFO_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SL_MOVIE_INFO");

            migrationBuilder.DropTable(
                name: "SL_TV_EPISODE_INFO");

            migrationBuilder.DropTable(
                name: "SL_TV_INFO");

            migrationBuilder.DropColumn(
                name: "INFO_ID",
                table: "SL_SHOW");

            migrationBuilder.DropColumn(
                name: "RESTART_BINGE",
                table: "SL_SHOW");
        }
    }
}
