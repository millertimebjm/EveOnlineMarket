using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveOnlineMarket.Migrations
{
    /// <inheritdoc />
    public partial class Schematic_Fix_Name_String : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SchematicName",
                table: "Schematic",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SchematicName",
                table: "Schematic",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
