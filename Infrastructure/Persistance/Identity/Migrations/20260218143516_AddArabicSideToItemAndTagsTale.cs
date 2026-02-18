using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddArabicSideToItemAndTagsTale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. تغيير أسماء الأعمدة (Name -> NameEn)
            migrationBuilder.RenameColumn(name: "Name", table: "RawMaterial", newName: "NameEn");
            migrationBuilder.RenameColumn(name: "Name", table: "Products", newName: "NameEn");

            // 2. إنشاء الـ Sequence
            migrationBuilder.CreateSequence(name: "ItemSequence");

            // 3. فك الـ Foreign Keys اللي "ماسكة" في الـ Id بتاع الجداول دي
            migrationBuilder.DropForeignKey(name: "FK_UserInteractions_Products_ProductId", table: "UserInteractions");
            migrationBuilder.DropForeignKey(name: "FK_UserInteractions_RawMaterial_RawMaterialId", table: "UserInteractions");
            migrationBuilder.DropForeignKey(name: "FK_Favourites_Products_ProductId", table: "Favourites");

            // 4. فك الـ Primary Keys (عشان نعرف نمسح عمود الـ Id نفسه)
            migrationBuilder.DropPrimaryKey(name: "PK_RawMaterial", table: "RawMaterial");
            migrationBuilder.DropPrimaryKey(name: "PK_Products", table: "Products");

            // 5. مسح أعمدة الـ Id القديمة (Identity)
            migrationBuilder.DropColumn(name: "Id", table: "RawMaterial");
            migrationBuilder.DropColumn(name: "Id", table: "Products");

            // 6. إضافة أعمدة الـ Id الجديدة (بتربط مع الـ Sequence)
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "RawMaterial",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR [ItemSequence]");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR [ItemSequence]");

            // 7. إعادة بناء الـ Primary Keys
            migrationBuilder.AddPrimaryKey(name: "PK_RawMaterial", table: "RawMaterial", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Products", table: "Products", column: "Id");

            // 8. إضافة أعمدة الـ BaseEntity والترجمة (NameAr)
            string[] tables = { "RawMaterial", "Products", "UserInteractions", "RawMaterialCategories", "ProductCategories", "Favourites" };
            foreach (var table in tables)
            {
                migrationBuilder.AddColumn<DateTime>(name: "CreatedAt", table: table, type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()");
                migrationBuilder.AddColumn<string>(name: "CreatedBy", table: table, type: "nvarchar(max)", nullable: false, defaultValue: "System");
                migrationBuilder.AddColumn<bool>(name: "IsDeleted", table: table, type: "bit", nullable: false, defaultValue: false);
                migrationBuilder.AddColumn<DateTime>(name: "UpdatedAt", table: table, type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()");
            }

            migrationBuilder.AddColumn<string>(name: "NameAr", table: "RawMaterial", type: "nvarchar(max)", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "NameAr", table: "Products", type: "nvarchar(max)", nullable: false, defaultValue: "");

            // 9. إنشاء جداول الـ Tags
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Tags", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "ItemTags",
                columns: table => new {
                    itemsId = table.Column<int>(type: "int", nullable: false),
                    tagsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_ItemTags", x => new { x.itemsId, x.tagsId });
                    table.ForeignKey(name: "FK_ItemTags_Tags_tagsId", column: x => x.tagsId, principalTable: "Tags", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
                });

            // 10. إعادة الـ Foreign Keys (بتربط الآن مع الـ Sequence Id)
            migrationBuilder.AddForeignKey(name: "FK_UserInteractions_Products_ProductId", table: "UserInteractions", column: "ProductId", principalTable: "Products", principalColumn: "Id");
            migrationBuilder.AddForeignKey(name: "FK_UserInteractions_RawMaterial_RawMaterialId", table: "UserInteractions", column: "RawMaterialId", principalTable: "RawMaterial", principalColumn: "Id");
            migrationBuilder.AddForeignKey(name: "FK_Favourites_Products_ProductId", table: "Favourites", column: "ProductId", principalTable: "Products", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
        }        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ItemTags");
            migrationBuilder.DropTable(name: "Tags");
            migrationBuilder.DropSequence(name: "ItemSequence");

            migrationBuilder.DropForeignKey(name: "FK_UserInteractions_Products_ProductId", table: "UserInteractions");
            migrationBuilder.DropForeignKey(name: "FK_UserInteractions_RawMaterial_RawMaterialId", table: "UserInteractions");
            migrationBuilder.DropForeignKey(name: "FK_Favourites_Products_ProductId", table: "Favourites");

            migrationBuilder.DropPrimaryKey(name: "PK_RawMaterial", table: "RawMaterial");
            migrationBuilder.DropPrimaryKey(name: "PK_Products", table: "Products");

            migrationBuilder.DropColumn(name: "Id", table: "RawMaterial");
            migrationBuilder.DropColumn(name: "Id", table: "Products");

            // إعادة الـ Identity
            migrationBuilder.AddColumn<int>(name: "Id", table: "RawMaterial", type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddColumn<int>(name: "Id", table: "Products", type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(name: "PK_RawMaterial", table: "RawMaterial", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Products", table: "Products", column: "Id");

            string[] tables = { "RawMaterial", "Products", "UserInteractions", "RawMaterialCategories", "ProductCategories", "Favourites" };
            foreach (var table in tables)
            {
                migrationBuilder.DropColumn(name: "CreatedAt", table: table);
                migrationBuilder.DropColumn(name: "CreatedBy", table: table);
                migrationBuilder.DropColumn(name: "IsDeleted", table: table);
                migrationBuilder.DropColumn(name: "UpdatedAt", table: table);
            }

            migrationBuilder.DropColumn(name: "NameAr", table: "RawMaterial");
            migrationBuilder.DropColumn(name: "NameAr", table: "Products");

            migrationBuilder.RenameColumn(name: "NameEn", table: "RawMaterial", newName: "Name");
            migrationBuilder.RenameColumn(name: "NameEn", table: "Products", newName: "Name");
        }
    }
}
