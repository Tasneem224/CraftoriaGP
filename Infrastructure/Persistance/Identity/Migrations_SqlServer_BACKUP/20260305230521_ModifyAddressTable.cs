using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class ModifyAddressTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Adress_Shipping");

            migrationBuilder.RenameColumn(
                name: "ShippingAddress_Street",
                table: "Orders",
                newName: "ShippingAddress_StreetDetails");

            migrationBuilder.RenameColumn(
                name: "ShippingAddress_State",
                table: "Orders",
                newName: "ShippingAddress_Region");

            migrationBuilder.RenameColumn(
                name: "ShippingAddress_Country",
                table: "Orders",
                newName: "ShippingAddress_PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Street",
                table: "Adress_Shipping",
                newName: "StreetDetails");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "Adress_Shipping",
                newName: "Region");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Adress_Shipping",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Adress_Shipping");

            migrationBuilder.RenameColumn(
                name: "ShippingAddress_StreetDetails",
                table: "Orders",
                newName: "ShippingAddress_Street");

            migrationBuilder.RenameColumn(
                name: "ShippingAddress_Region",
                table: "Orders",
                newName: "ShippingAddress_State");

            migrationBuilder.RenameColumn(
                name: "ShippingAddress_PhoneNumber",
                table: "Orders",
                newName: "ShippingAddress_Country");

            migrationBuilder.RenameColumn(
                name: "StreetDetails",
                table: "Adress_Shipping",
                newName: "Street");

            migrationBuilder.RenameColumn(
                name: "Region",
                table: "Adress_Shipping",
                newName: "Country");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Adress_Shipping",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
