using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveOnlineMarket.Migrations
{
    /// <inheritdoc />
    public partial class EveType_SchematicId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SchematicId",
                table: "Type",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SchematicId",
                table: "Type");
        }
    }
}
