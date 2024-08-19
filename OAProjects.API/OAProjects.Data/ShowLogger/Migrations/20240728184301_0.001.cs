using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.ShowLogger.Migrations
{
    public partial class _0001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SL_BOOK",
                columns: table => new
                {
                    BOOK_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USER_ID = table.Column<int>(type: "int", nullable: false),
                    BOOK_NAME = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    START_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    END_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CHAPTERS = table.Column<int>(type: "int", nullable: true),
                    PAGES = table.Column<int>(type: "int", nullable: true),
                    BOOK_NOTES = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_BOOK", x => x.BOOK_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SL_CODE_VALUE",
                columns: table => new
                {
                    CODE_VALUE_ID = table.Column<int>(type: "int", nullable: false),
                    CODE_TABLE_ID = table.Column<int>(type: "int", nullable: false),
                    DECODE_TXT = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EXTRA_INFO = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_CODE_VALUE", x => x.CODE_VALUE_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SL_FRIEND",
                columns: table => new
                {
                    FRIEND_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USER_ID = table.Column<int>(type: "int", nullable: false),
                    FRIEND_USER_ID = table.Column<int>(type: "int", nullable: false),
                    CREATED_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_FRIEND", x => x.FRIEND_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SL_FRIEND_REQUEST",
                columns: table => new
                {
                    FRIEND_REQUEST_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SENT_USER_ID = table.Column<int>(type: "int", nullable: false),
                    RECEIVED_USER_ID = table.Column<int>(type: "int", nullable: false),
                    DATE_SENT = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_FRIEND_REQUEST", x => x.FRIEND_REQUEST_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SL_ID_XREF",
                columns: table => new
                {
                    ID_XREF_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TABLE_ID = table.Column<int>(type: "int", nullable: false),
                    OLD_ID = table.Column<int>(type: "int", nullable: false),
                    NEW_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_ID_XREF", x => x.ID_XREF_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SL_MOVIE_INFO",
                columns: table => new
                {
                    MOVIE_INFO_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MOVIE_NAME = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MOVIE_OVERVIEW = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    API_TYPE = table.Column<int>(type: "int", nullable: true),
                    API_ID = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RUNTIME = table.Column<int>(type: "int", nullable: true),
                    AIR_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LAST_DATA_REFRESH = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LAST_UPDATED = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    POSTER_URL = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BACKDROP_URL = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_MOVIE_INFO", x => x.MOVIE_INFO_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SL_SHOW",
                columns: table => new
                {
                    SHOW_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USER_ID = table.Column<int>(type: "int", nullable: false),
                    SHOW_NAME = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SHOW_TYPE_ID = table.Column<int>(type: "int", nullable: false),
                    SEASON_NUMBER = table.Column<int>(type: "int", nullable: true),
                    EPISODE_NUMBER = table.Column<int>(type: "int", nullable: true),
                    DATE_WATCHED = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SHOW_NOTES = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RESTART_BINGE = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    INFO_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_SHOW", x => x.SHOW_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SL_TRANSACTION",
                columns: table => new
                {
                    TRANSACTION_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USER_ID = table.Column<int>(type: "int", nullable: false),
                    TRANSACTION_TYPE_ID = table.Column<int>(type: "int", nullable: false),
                    SHOW_ID = table.Column<int>(type: "int", nullable: true),
                    ITEM = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    COST_AMT = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: false),
                    TRANSACTION_NOTES = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TRANSACTION_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_TRANSACTION", x => x.TRANSACTION_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SL_TV_EPISODE_ORDER",
                columns: table => new
                {
                    TV_EPISODE_ORDER_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TV_INFO_ID = table.Column<int>(type: "int", nullable: false),
                    TV_EPISODE_INFO_ID = table.Column<int>(type: "int", nullable: false),
                    EPISODE_ORDER = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_TV_EPISODE_ORDER", x => x.TV_EPISODE_ORDER_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SL_TV_INFO",
                columns: table => new
                {
                    TV_INFO_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SHOW_NAME = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SHOW_OVERVIEW = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    API_TYPE = table.Column<int>(type: "int", nullable: true),
                    API_ID = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LAST_DATA_REFRESH = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LAST_UPDATED = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    POSTER_URL = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BACKDROP_URL = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    STATUS = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_TV_INFO", x => x.TV_INFO_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SL_USER_PREF",
                columns: table => new
                {
                    USER_PREF_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USER_ID = table.Column<int>(type: "int", nullable: false),
                    DEFAULT_AREA = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_USER_PREF", x => x.USER_PREF_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SL_WATCHLIST",
                columns: table => new
                {
                    WATCHLIST_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USER_ID = table.Column<int>(type: "int", nullable: false),
                    SHOW_NAME = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SHOW_TYPE_ID = table.Column<int>(type: "int", nullable: false),
                    SEASON_NUMBER = table.Column<int>(type: "int", nullable: true),
                    EPISODE_NUMBER = table.Column<int>(type: "int", nullable: true),
                    DATE_ADDED = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SHOW_NOTES = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    INFO_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_WATCHLIST", x => x.WATCHLIST_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SL_TV_EPISODE_INFO",
                columns: table => new
                {
                    TV_EPISODE_INFO_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TV_INFO_ID = table.Column<int>(type: "int", nullable: false),
                    API_TYPE = table.Column<int>(type: "int", nullable: true),
                    API_ID = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SEASON_NAME = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EPISODE_NAME = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SEASON_NUMBER = table.Column<int>(type: "int", nullable: true),
                    EPISODE_NUMBER = table.Column<int>(type: "int", nullable: true),
                    EPISODE_OVERVIEW = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RUNTIME = table.Column<int>(type: "int", nullable: true),
                    AIR_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IMAGE_URL = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                    { 2003, 2, "AMC A-list", null },
                    { 2004, 2, "Benefits", null },
                    { 2005, 2, "Rewards", null },
                    { 2006, 2, "Tax", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SL_TV_EPISODE_INFO_TV_INFO_ID",
                table: "SL_TV_EPISODE_INFO",
                column: "TV_INFO_ID");

            migrationBuilder.Sql(@"
CREATE VIEW sl_year_stats_data_vw
AS
SELECT x.user_id
	  ,x.show_name
	  ,YEAR(x.date_watched) year
	  ,x.show_type_id
	  ,CASE WHEN x.show_type_id = 1000 THEN SUM(ei.runtime)
			ELSE SUM(mi.runtime)
	   END total_runtime
	  ,CASE WHEN x.show_type_id = 1000 THEN ti.api_type
			ELSE mi.api_type
	   END api_type
	  ,CASE WHEN x.show_type_id = 1000 THEN ti.api_id
			ELSE mi.api_id
	   END api_id
	  ,CASE WHEN x.show_type_id = 1000 THEN ti.backdrop_url
			ELSE mi.backdrop_url
	   END backdrop_url
	  ,COUNT(*) watch_count
FROM   sl_show x
LEFT OUTER JOIN sl_tv_episode_info ei
	ON (ei.tv_episode_info_id = x.info_id)
LEFT OUTER JOIN sl_tv_info ti
	ON (ti.tv_info_id = ei.tv_info_id)
LEFT OUTER JOIN sl_movie_info mi
	ON (mi.movie_info_id = x.info_id)
GROUP BY x.user_id
	  ,x.show_name
	  ,YEAR(x.date_watched)
	  ,x.show_type_id
	  ,CASE WHEN x.show_type_id = 1000 THEN ti.api_type
			ELSE mi.api_type
	   END
	  ,CASE WHEN x.show_type_id = 1000 THEN ti.api_id
			ELSE mi.api_id
	   END
	  ,CASE WHEN x.show_type_id = 1000 THEN ti.backdrop_url
			ELSE mi.backdrop_url
	   END
;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SL_BOOK");

            migrationBuilder.DropTable(
                name: "SL_CODE_VALUE");

            migrationBuilder.DropTable(
                name: "SL_FRIEND");

            migrationBuilder.DropTable(
                name: "SL_FRIEND_REQUEST");

            migrationBuilder.DropTable(
                name: "SL_ID_XREF");

            migrationBuilder.DropTable(
                name: "SL_MOVIE_INFO");

            migrationBuilder.DropTable(
                name: "SL_SHOW");

            migrationBuilder.DropTable(
                name: "SL_TRANSACTION");

            migrationBuilder.DropTable(
                name: "SL_TV_EPISODE_INFO");

            migrationBuilder.DropTable(
                name: "SL_TV_EPISODE_ORDER");

            migrationBuilder.DropTable(
                name: "SL_USER_PREF");

            migrationBuilder.DropTable(
                name: "SL_WATCHLIST");

            migrationBuilder.DropTable(
                name: "SL_TV_INFO");

            migrationBuilder.Sql(@"
DROP VIEW sl_year_stats_data_vw;
");
        }
    }
}
