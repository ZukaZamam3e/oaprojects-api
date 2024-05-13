using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.ShowLogger.Migrations
{
    public partial class _0011 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SL_TV_EPISODE_ORDER",
                columns: table => new
                {
                    TV_EPISODE_ORDER_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TV_INFO_ID = table.Column<int>(type: "int", nullable: false),
                    TV_EPISODE_INFO_ID = table.Column<int>(type: "int", nullable: false),
                    EPISODE_ORDER = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_TV_EPISODE_ORDER", x => x.TV_EPISODE_ORDER_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SL_TV_EPISODE_ORDER");
        }
    }
}
