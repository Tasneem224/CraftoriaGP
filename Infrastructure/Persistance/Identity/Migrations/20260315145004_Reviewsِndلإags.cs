using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class Reviewsِndلإags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM ItemTags;");
            migrationBuilder.DropForeignKey(
                name: "FK_ItemTags_Tags_tagsId",
                table: "ItemTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemTags",
                table: "ItemTags");

            //migrationBuilder.DropIndex(
            //    name: "IX_ItemTags_tagsId",
            //    table: "ItemTags");

            migrationBuilder.RenameColumn(
                name: "tagsId",
                table: "ItemTags",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "itemsId",
                table: "ItemTags",
                newName: "TagId");

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "ItemTags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ItemTags",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ItemTags",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ItemTags",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "ItemTags",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ItemTags",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemTags",
                table: "ItemTags",
                columns: new[] { "ItemId", "TagId" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemTags_TagId",
                table: "ItemTags",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTags_Tags_TagId",
                table: "ItemTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_ItemTags_Tags_TagId",
                table: "ItemTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemTags",
                table: "ItemTags");

            migrationBuilder.DropIndex(
                name: "IX_ItemTags_TagId",
                table: "ItemTags");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "ItemTags");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ItemTags");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ItemTags");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ItemTags");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "ItemTags");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ItemTags");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ItemTags",
                newName: "tagsId");

            migrationBuilder.RenameColumn(
                name: "TagId",
                table: "ItemTags",
                newName: "itemsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemTags",
                table: "ItemTags",
                columns: new[] { "itemsId", "tagsId" });

            //migrationBuilder.CreateIndex(
            //    name: "IX_ItemTags_tagsId",
            //    table: "ItemTags",
            //    column: "tagsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTags_Tags_tagsId",
                table: "ItemTags",
                column: "tagsId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
