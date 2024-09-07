using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OAProjects.Data.ShowLogger.Migrations
{
    /// <inheritdoc />
    public partial class _1001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SL_WHATS_NEXT_SUB",
                columns: table => new
                {
                    WHATS_NEXT_SUB_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USER_ID = table.Column<int>(type: "int", nullable: false),
                    TV_INFO_ID = table.Column<int>(type: "int", nullable: false),
                    INCLUDE_SPECIALS = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SUBSCRIBE_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SL_WHATS_NEXT_SUB", x => x.WHATS_NEXT_SUB_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SL_WHATS_NEXT_SUB");
        }
    }
}
