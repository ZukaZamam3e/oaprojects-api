using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.FinanceTracker.Migrations
{
    /// <inheritdoc />
    public partial class _0005 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CONDITIONAL",
                table: "FT_TRANSACTION",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CONDITIONAL_AMOUNT",
                table: "FT_TRANSACTION",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "FT_CODE_VALUE",
                columns: new[] { "CODE_VALUE_ID", "CODE_TABLE_ID", "DECODE_TXT", "EXTRA_INFO" },
                values: new object[] { 2000, 2, "3/Month", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FT_CODE_VALUE",
                keyColumn: "CODE_VALUE_ID",
                keyValue: 2000);

            migrationBuilder.DropColumn(
                name: "CONDITIONAL",
                table: "FT_TRANSACTION");

            migrationBuilder.DropColumn(
                name: "CONDITIONAL_AMOUNT",
                table: "FT_TRANSACTION");
        }
    }
}
