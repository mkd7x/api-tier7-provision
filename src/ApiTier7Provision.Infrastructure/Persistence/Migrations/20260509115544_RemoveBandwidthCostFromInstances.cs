using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiTier7Provision.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBandwidthCostFromInstances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bandwidth_cost",
                table: "instances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "bandwidth_cost",
                table: "instances",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
