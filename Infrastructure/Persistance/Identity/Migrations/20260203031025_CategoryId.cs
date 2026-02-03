using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class CategoryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "RawMaterial",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RawMaterial_CategoryId",
                table: "RawMaterial",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RawMaterial_RawMaterialCategories_CategoryId",
                table: "RawMaterial",
                column: "CategoryId",
                principalTable: "RawMaterialCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterial_RawMaterialCategories_CategoryId",
                table: "RawMaterial");

            migrationBuilder.DropIndex(
                name: "IX_RawMaterial_CategoryId",
                table: "RawMaterial");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "RawMaterial");
        }
    }
}
