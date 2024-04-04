using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.ShowLogger.Migrations
{
    public partial class _0005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OTHER_NAMES",
                table: "SL_TV_INFO");

            migrationBuilder.DropColumn(
                name: "OTHER_NAMES",
                table: "SL_MOVIE_INFO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OTHER_NAMES",
                table: "SL_TV_INFO",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OTHER_NAMES",
                table: "SL_MOVIE_INFO",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
