using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OAProjects.Data.FinanceTracker.Migrations
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
                name: "FT_ACCOUNT",
                columns: table => new
                {
                    ACCOUNT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USER_ID = table.Column<int>(type: "int", nullable: false),
                    ACCOUNT_NAME = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IS_DEFAULT = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FT_ACCOUNT", x => x.ACCOUNT_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FT_CODE_VALUE",
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
                    table.PrimaryKey("PK_FT_CODE_VALUE", x => x.CODE_VALUE_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FT_TRANSACTION",
                columns: table => new
                {
                    TRANSACTION_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ACCOUNT_ID = table.Column<int>(type: "int", nullable: false),
                    TRANSACTION_NAME = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    START_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    END_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TRANSACTION_AMOUNT = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    FREQUENCY_TYPE_ID = table.Column<int>(type: "int", nullable: false),
                    TRANSACTION_NOTES = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TRANSACTION_URL = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FT_TRANSACTION", x => x.TRANSACTION_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FT_TRANSACTION_OFFSET",
                columns: table => new
                {
                    TRANSACTION_OFFSET_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TRANSACTION_ID = table.Column<int>(type: "int", nullable: false),
                    OFFSET_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    OFFSET_AMOUNT = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FT_TRANSACTION_OFFSET", x => x.TRANSACTION_OFFSET_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "FT_CODE_VALUE",
                columns: new[] { "CODE_VALUE_ID", "CODE_TABLE_ID", "DECODE_TXT", "EXTRA_INFO" },
                values: new object[,]
                {
                    { 1000, 1, "Hardset", null },
                    { 1001, 1, "Single", null },
                    { 1002, 1, "Daily", null },
                    { 1003, 1, "Weekly", null },
                    { 1004, 1, "Bi-Weekly", null },
                    { 1005, 1, "Every Four Weeks", null },
                    { 1006, 1, "Monthly", null },
                    { 1007, 1, "Quarterly", null },
                    { 1008, 1, "Yearly", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FT_ACCOUNT");

            migrationBuilder.DropTable(
                name: "FT_CODE_VALUE");

            migrationBuilder.DropTable(
                name: "FT_TRANSACTION");

            migrationBuilder.DropTable(
                name: "FT_TRANSACTION_OFFSET");
        }
    }
}
