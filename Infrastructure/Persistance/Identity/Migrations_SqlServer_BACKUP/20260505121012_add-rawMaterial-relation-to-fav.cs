using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class addrawMaterialrelationtofav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favourites_Products_ProductId",
                table: "Favourites");

            migrationBuilder.DropIndex(
                name: "IX_Favourites_UserId_ProductId",
                table: "Favourites");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Favourites",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "RawMaterialId",
                table: "Favourites",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_RawMaterialId",
                table: "Favourites",
                column: "RawMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_UserId_ProductId",
                table: "Favourites",
                columns: new[] { "UserId", "ProductId" },
                unique: true,
                filter: "[ProductId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Favourites_Products_ProductId",
                table: "Favourites",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favourites_RawMaterials_RawMaterialId",
                table: "Favourites",
                column: "RawMaterialId",
                principalTable: "RawMaterials",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favourites_Products_ProductId",
                table: "Favourites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favourites_RawMaterials_RawMaterialId",
                table: "Favourites");

            migrationBuilder.DropIndex(
                name: "IX_Favourites_RawMaterialId",
                table: "Favourites");

            migrationBuilder.DropIndex(
                name: "IX_Favourites_UserId_ProductId",
                table: "Favourites");

            migrationBuilder.DropColumn(
                name: "RawMaterialId",
                table: "Favourites");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Favourites",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_UserId_ProductId",
                table: "Favourites",
                columns: new[] { "UserId", "ProductId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Favourites_Products_ProductId",
                table: "Favourites",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
