using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.ShowLogger.Migrations
{
    public partial class _0012 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IMAGE_URL",
                table: "SL_TV_INFO",
                newName: "POSTER_URL");

            migrationBuilder.AddColumn<string>(
                name: "BACKDROP_URL",
                table: "SL_TV_INFO",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BACKDROP_URL",
                table: "SL_TV_INFO");

            migrationBuilder.RenameColumn(
                name: "POSTER_URL",
                table: "SL_TV_INFO",
                newName: "IMAGE_URL");
        }
    }
}
