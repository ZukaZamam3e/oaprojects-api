using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.ShowLogger.Migrations
{
    public partial class _0013 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "INFO_ID",
                table: "SL_WATCHLIST",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "INFO_ID",
                table: "SL_WATCHLIST");
        }
    }
}
