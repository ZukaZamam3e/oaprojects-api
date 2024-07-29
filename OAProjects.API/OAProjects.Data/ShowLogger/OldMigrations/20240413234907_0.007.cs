using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.ShowLogger.Migrations
{
    public partial class _0007 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SL_ID_XREF",
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
                    table.PrimaryKey("PK_SL_ID_XREF", x => x.ID_XREF_ID);
                });

            migrationBuilder.UpdateData(
                table: "SL_CODE_VALUE",
                keyColumn: "CODE_VALUE_ID",
                keyValue: 2005,
                column: "DECODE_TXT",
                value: "Rewards");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SL_ID_XREF");

            migrationBuilder.UpdateData(
                table: "SL_CODE_VALUE",
                keyColumn: "CODE_VALUE_ID",
                keyValue: 2005,
                column: "DECODE_TXT",
                value: "Discount");
        }
    }
}
