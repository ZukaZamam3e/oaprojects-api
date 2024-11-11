using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OAProjects.Data.FinanceTracker.Migrations
{
    /// <inheritdoc />
    public partial class _0003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "INTERVAL",
                table: "FT_TRANSACTION",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "FT_CODE_VALUE",
                columns: new[] { "CODE_VALUE_ID", "CODE_TABLE_ID", "DECODE_TXT", "EXTRA_INFO" },
                values: new object[,]
                {
                    { 1009, 1, "Every N Days", null },
                    { 1010, 1, "Every N Weeks", null },
                    { 1011, 1, "Every N Months", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FT_CODE_VALUE",
                keyColumn: "CODE_VALUE_ID",
                keyValue: 1009);

            migrationBuilder.DeleteData(
                table: "FT_CODE_VALUE",
                keyColumn: "CODE_VALUE_ID",
                keyValue: 1010);

            migrationBuilder.DeleteData(
                table: "FT_CODE_VALUE",
                keyColumn: "CODE_VALUE_ID",
                keyValue: 1011);

            migrationBuilder.DropColumn(
                name: "INTERVAL",
                table: "FT_TRANSACTION");
        }
    }
}
