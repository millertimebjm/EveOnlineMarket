using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveOnlineMarket.Migrations
{
    /// <inheritdoc />
    public partial class Added_EveUniverseType_MarketGroupId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MarketGroupId",
                table: "Type",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarketGroupId",
                table: "Type");
        }
    }
}
