using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project_itransition.Migrations
{
    /// <inheritdoc />
    public partial class AddConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Items_InventoryId",
                table: "Items");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Items",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Inventories",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_InventoryId_CustomId",
                table: "Items",
                columns: new[] { "InventoryId", "CustomId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Items_InventoryId_CustomId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Inventories");

            migrationBuilder.CreateIndex(
                name: "IX_Items_InventoryId",
                table: "Items",
                column: "InventoryId");
        }
    }
}
