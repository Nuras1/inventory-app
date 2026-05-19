using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project_itransition.Migrations
{
    /// <inheritdoc />
    public partial class FixAddInventoryAccessNew3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CustomId",
                table: "Items",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "CustomIdFormat",
                table: "Inventories",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomIdFormat",
                table: "Inventories");

            migrationBuilder.AlterColumn<string>(
                name: "CustomId",
                table: "Items",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
