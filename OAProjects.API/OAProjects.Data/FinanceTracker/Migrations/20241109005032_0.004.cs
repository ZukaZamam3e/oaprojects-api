using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.FinanceTracker.Migrations
{
    /// <inheritdoc />
    public partial class _0004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CATEGORIES",
                table: "FT_TRANSACTION",
                type: "varchar(250)",
                maxLength: 250,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CATEGORIES",
                table: "FT_TRANSACTION");
        }
    }
}
