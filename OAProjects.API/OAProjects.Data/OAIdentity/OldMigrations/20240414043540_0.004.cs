using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.OAIdentity.Migrations
{
    public partial class _0004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OA_ID_XREF",
                columns: table => new
                {
                    ID_XREF_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TABLE_ID = table.Column<int>(type: "int", nullable: false),
                    OLD_ID = table.Column<int>(type: "int", nullable: false),
                    NEW_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OA_ID_XREF", x => x.ID_XREF_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OA_ID_XREF");
        }
    }
}
