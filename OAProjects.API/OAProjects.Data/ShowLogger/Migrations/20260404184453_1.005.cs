using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.ShowLogger.Migrations
{
    /// <inheritdoc />
    public partial class _1005 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SL_CODE_VALUE",
                columns: new[] { "CODE_VALUE_ID", "CODE_TABLE_ID", "DECODE_TXT", "EXTRA_INFO" },
                values: new object[] { 2007, 2, "Popcorn Pass", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SL_CODE_VALUE",
                keyColumn: "CODE_VALUE_ID",
                keyValue: 2007);
        }
    }
}
