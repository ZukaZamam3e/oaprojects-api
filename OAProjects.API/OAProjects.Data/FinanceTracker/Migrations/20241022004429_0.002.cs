using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.FinanceTracker.Migrations
{
    /// <inheritdoc />
    public partial class _0002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IS_DEFAULT",
                table: "FT_ACCOUNT",
                newName: "DEFAULT_INDC");

            migrationBuilder.AddColumn<int>(
                name: "ACCOUNT_ID",
                table: "FT_TRANSACTION_OFFSET",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "USER_ID",
                table: "FT_TRANSACTION_OFFSET",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "USER_ID",
                table: "FT_TRANSACTION",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FT_ID_XREF",
                columns: table => new
                {
                    ID_XREF_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TABLE_ID = table.Column<int>(type: "int", nullable: false),
                    OLD_ID = table.Column<int>(type: "int", nullable: false),
                    NEW_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FT_ID_XREF", x => x.ID_XREF_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FT_ID_XREF");

            migrationBuilder.DropColumn(
                name: "ACCOUNT_ID",
                table: "FT_TRANSACTION_OFFSET");

            migrationBuilder.DropColumn(
                name: "USER_ID",
                table: "FT_TRANSACTION_OFFSET");

            migrationBuilder.DropColumn(
                name: "USER_ID",
                table: "FT_TRANSACTION");

            migrationBuilder.RenameColumn(
                name: "DEFAULT_INDC",
                table: "FT_ACCOUNT",
                newName: "IS_DEFAULT");
        }
    }
}
