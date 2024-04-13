using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.ShowLogger.Migrations
{
    public partial class _0006 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SL_TRANSACTION",
                columns: table => new
                {
                    TRANSACTION_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USER_ID = table.Column<int>(type: "int", nullable: false),
                    TRANSACTION_TYPE_ID = table.Column<int>(type: "int", nullable: false),
                    SHOW_ID = table.Column<int>(type: "int", nullable: true),
                    ITEM = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    COST_AMT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: false),
                    TRANSACTION_NOTES = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TRANSACTION_DATE = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_TRANSACTION", x => x.TRANSACTION_ID);
                });

            migrationBuilder.InsertData(
                table: "SL_CODE_VALUE",
                columns: new[] { "CODE_VALUE_ID", "CODE_TABLE_ID", "DECODE_TXT", "EXTRA_INFO" },
                values: new object[] { 2004, 2, "Benefits", null });

            migrationBuilder.InsertData(
                table: "SL_CODE_VALUE",
                columns: new[] { "CODE_VALUE_ID", "CODE_TABLE_ID", "DECODE_TXT", "EXTRA_INFO" },
                values: new object[] { 2005, 2, "Rewards", null });

            migrationBuilder.InsertData(
                table: "SL_CODE_VALUE",
                columns: new[] { "CODE_VALUE_ID", "CODE_TABLE_ID", "DECODE_TXT", "EXTRA_INFO" },
                values: new object[] { 2006, 2, "Tax", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SL_TRANSACTION");

            migrationBuilder.DeleteData(
                table: "SL_CODE_VALUE",
                keyColumn: "CODE_VALUE_ID",
                keyValue: 2004);

            migrationBuilder.DeleteData(
                table: "SL_CODE_VALUE",
                keyColumn: "CODE_VALUE_ID",
                keyValue: 2005);

            migrationBuilder.DeleteData(
                table: "SL_CODE_VALUE",
                keyColumn: "CODE_VALUE_ID",
                keyValue: 2006);
        }
    }
}
