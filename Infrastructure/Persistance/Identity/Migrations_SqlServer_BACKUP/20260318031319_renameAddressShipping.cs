using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class renameAddressShipping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Adress_Shipping",
                table: "Adress_Shipping");

            migrationBuilder.RenameTable(
                name: "Adress_Shipping",
                newName: "AddressBooks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AddressBooks",
                table: "AddressBooks",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AddressBooks",
                table: "AddressBooks");

            migrationBuilder.RenameTable(
                name: "AddressBooks",
                newName: "Adress_Shipping");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Adress_Shipping",
                table: "Adress_Shipping",
                column: "Id");
        }
    }
}
