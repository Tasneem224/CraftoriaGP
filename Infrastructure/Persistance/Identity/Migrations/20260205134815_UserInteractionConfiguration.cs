using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class UserInteractionConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserInteractions_UserId",
                table: "UserInteractions");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractions_UserId_ProductId",
                table: "UserInteractions",
                columns: new[] { "UserId", "ProductId" },
                unique: true,
                filter: "[ProductId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractions_UserId_RawMaterialId",
                table: "UserInteractions",
                columns: new[] { "UserId", "RawMaterialId" },
                unique: true,
                filter: "[RawMaterialId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractions_UserId_TargetUserId",
                table: "UserInteractions",
                columns: new[] { "UserId", "TargetUserId" },
                unique: true,
                filter: "[TargetUserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserInteractions_UserId_ProductId",
                table: "UserInteractions");

            migrationBuilder.DropIndex(
                name: "IX_UserInteractions_UserId_RawMaterialId",
                table: "UserInteractions");

            migrationBuilder.DropIndex(
                name: "IX_UserInteractions_UserId_TargetUserId",
                table: "UserInteractions");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractions_UserId",
                table: "UserInteractions",
                column: "UserId");
        }
    }
}
