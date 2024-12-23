using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WholesaleBase.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalCostToOrderandAPPCONTEXT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "Orders");
        }
    }
}
