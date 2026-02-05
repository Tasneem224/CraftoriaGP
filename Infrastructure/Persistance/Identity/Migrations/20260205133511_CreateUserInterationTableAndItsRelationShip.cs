using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserInterationTableAndItsRelationShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<int>(
            //    name: "CategoryId",
            //    table: "RawMaterial",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserInteractions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    RawMaterialId = table.Column<int>(type: "int", nullable: true),
                    TargetUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsFavourite = table.Column<bool>(type: "bit", nullable: true),
                    Rating = table.Column<short>(type: "smallint", nullable: true),
                    Review = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InteractionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInteractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInteractions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserInteractions_RawMaterial_RawMaterialId",
                        column: x => x.RawMaterialId,
                        principalTable: "RawMaterial",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserInteractions_Users_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserInteractions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateIndex(
            //    name: "IX_RawMaterial_CategoryId",
            //    table: "RawMaterial",
            //    column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractions_ProductId",
                table: "UserInteractions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractions_RawMaterialId",
                table: "UserInteractions",
                column: "RawMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractions_TargetUserId",
                table: "UserInteractions",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractions_UserId",
                table: "UserInteractions",
                column: "UserId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_RawMaterial_RawMaterialCategories_CategoryId",
            //    table: "RawMaterial",
            //    column: "CategoryId",
            //    principalTable: "RawMaterialCategories",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterial_RawMaterialCategories_CategoryId",
                table: "RawMaterial");

            migrationBuilder.DropTable(
                name: "UserInteractions");

            migrationBuilder.DropIndex(
                name: "IX_RawMaterial_CategoryId",
                table: "RawMaterial");

            //migrationBuilder.DropColumn(
            //    name: "CategoryId",
            //    table: "RawMaterial");
        }
    }
}
