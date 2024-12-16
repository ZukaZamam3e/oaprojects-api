using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.CatanLogger.Migrations
{
    /// <inheritdoc />
    public partial class _0001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CL_GROUP",
                columns: table => new
                {
                    GROUP_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GROUP_NAME = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DATE_ADDED = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CL_GROUP", x => x.GROUP_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CL_GAME",
                columns: table => new
                {
                    GAME_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GROUP_ID = table.Column<int>(type: "int", nullable: false),
                    DATE_PLAYED = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TURN_ORDER = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GAME_DELETED = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CL_GAME", x => x.GAME_ID);
                    table.ForeignKey(
                        name: "FK_CL_GAME_CL_GROUP_GROUP_ID",
                        column: x => x.GROUP_ID,
                        principalTable: "CL_GROUP",
                        principalColumn: "GROUP_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CL_GROUP_USER",
                columns: table => new
                {
                    GROUP_USER_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GROUP_ID = table.Column<int>(type: "int", nullable: false),
                    USER_ID = table.Column<int>(type: "int", nullable: false),
                    ROLE_ID = table.Column<int>(type: "int", nullable: false),
                    DATE_ADDED = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GROUP_USER_STATUS = table.Column<int>(type: "int", nullable: false),
                    CONFIRMED_USER_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CL_GROUP_USER", x => x.GROUP_USER_ID);
                    table.ForeignKey(
                        name: "FK_CL_GROUP_USER_CL_GROUP_GROUP_ID",
                        column: x => x.GROUP_ID,
                        principalTable: "CL_GROUP",
                        principalColumn: "GROUP_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CL_DICEROLL",
                columns: table => new
                {
                    DICE_ROLL_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GAME_ID = table.Column<int>(type: "int", nullable: false),
                    DICE_NUMBER = table.Column<int>(type: "int", nullable: false),
                    DICE_ROLLS = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CL_DICEROLL", x => x.DICE_ROLL_ID);
                    table.ForeignKey(
                        name: "FK_CL_DICEROLL_CL_GAME_GAME_ID",
                        column: x => x.GAME_ID,
                        principalTable: "CL_GAME",
                        principalColumn: "GAME_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CL_PLAYER",
                columns: table => new
                {
                    PLAYER_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GAME_ID = table.Column<int>(type: "int", nullable: false),
                    PLAYER_NAME = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PLAYER_COLOR = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WINNER = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IS_PLAYING = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CL_PLAYER", x => x.PLAYER_ID);
                    table.ForeignKey(
                        name: "FK_CL_PLAYER_CL_GAME_GAME_ID",
                        column: x => x.GAME_ID,
                        principalTable: "CL_GAME",
                        principalColumn: "GAME_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CL_DICEROLL_GAME_ID",
                table: "CL_DICEROLL",
                column: "GAME_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CL_GAME_GROUP_ID",
                table: "CL_GAME",
                column: "GROUP_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CL_GROUP_USER_GROUP_ID",
                table: "CL_GROUP_USER",
                column: "GROUP_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CL_PLAYER_GAME_ID",
                table: "CL_PLAYER",
                column: "GAME_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CL_DICEROLL");

            migrationBuilder.DropTable(
                name: "CL_GROUP_USER");

            migrationBuilder.DropTable(
                name: "CL_PLAYER");

            migrationBuilder.DropTable(
                name: "CL_GAME");

            migrationBuilder.DropTable(
                name: "CL_GROUP");
        }
    }
}
