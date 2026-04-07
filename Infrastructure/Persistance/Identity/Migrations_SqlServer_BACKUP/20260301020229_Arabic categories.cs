using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class Arabiccategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "RawMaterialCategories",
                newName: "NameEn");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductCategories",
                newName: "NameEn");

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "RawMaterialCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "ProductCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "RawMaterialCategories");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "ProductCategories");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "RawMaterialCategories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "ProductCategories",
                newName: "Name");
        }
    }
}
