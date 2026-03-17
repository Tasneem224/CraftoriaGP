using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddIsGeneratedToTallTablesThatInheriteFromBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "UserInteractions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "Tags",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "Sessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "RawMaterials",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "RawMaterialCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "ProductCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "OrderItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "Favourites",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "ExpertServices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "ExpertAvailabilities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "DeliveryMethods",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "Adress_Shipping",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "UserInteractions");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "RawMaterials");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "RawMaterialCategories");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "Favourites");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "ExpertServices");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "ExpertAvailabilities");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "DeliveryMethods");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "Adress_Shipping");
        }
    }
}
