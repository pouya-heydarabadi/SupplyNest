using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupplyNest.Inventory.Api.Infrastructure.SqlServerConfig.Migrations
{
    /// <inheritdoc />
    public partial class changeinventoryforkeyandindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventories",
                table: "Inventories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventories",
                table: "Inventories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_WarehouseId_ProductId_FiscalYearId",
                table: "Inventories",
                columns: new[] { "WarehouseId", "ProductId", "FiscalYearId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventories",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_WarehouseId_ProductId_FiscalYearId",
                table: "Inventories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventories",
                table: "Inventories",
                columns: new[] { "WarehouseId", "ProductId", "FiscalYearId" });
        }
    }
}
