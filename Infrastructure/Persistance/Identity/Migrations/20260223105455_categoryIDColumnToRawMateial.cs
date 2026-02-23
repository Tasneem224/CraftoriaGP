using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class categoryIDColumnToRawMateial : Migration
    {
        
            protected override void Up(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.DropForeignKey(
                    name: "FK_UserInteractions_RawMaterial_RawMaterialId",
                    table: "UserInteractions");

                // 2. مسح عمود IsFavourite من UserInteractions
                migrationBuilder.DropColumn(
                    name: "IsFavourite",
                    table: "UserInteractions");

                // 3. دلوقتي نقدر نمسح الـ Primary Key بأمان
                migrationBuilder.DropPrimaryKey(
                    name: "PK_RawMaterial",
                    table: "RawMaterial");

                // 4. تغيير اسم الجدول للجمع
                migrationBuilder.RenameTable(
                    name: "RawMaterial",
                    newName: "RawMaterials");

                // 5. إضافة الـ Primary Key للاسم الجديد
                migrationBuilder.AddPrimaryKey(
                    name: "PK_RawMaterials",
                    table: "RawMaterials",
                    column: "Id");

                // 6. إضافة الأعمدة اللي كانت ناقصة (المشكلة الأساسية)
                migrationBuilder.AddColumn<int>(
                    name: "CategoryId",
                    table: "RawMaterials",
                    type: "int",
                    nullable: false,
                    defaultValue: 1);

             
                // 7. إنشاء الـ Indexes
                migrationBuilder.CreateIndex(
                    name: "IX_RawMaterials_CategoryId",
                    table: "RawMaterials",
                    column: "CategoryId");

                migrationBuilder.CreateIndex(
                    name: "IX_RawMaterials_supplierId",
                    table: "RawMaterials",
                    column: "supplierId");

                // 8. نرجع نربط كل العلاقات تاني بسلام
                migrationBuilder.AddForeignKey(
                    name: "FK_UserInteractions_RawMaterials_RawMaterialId",
                    table: "UserInteractions",
                    column: "RawMaterialId",
                    principalTable: "RawMaterials",
                    principalColumn: "Id");

                migrationBuilder.AddForeignKey(
                    name: "FK_RawMaterials_RawMaterialCategories_CategoryId",
                    table: "RawMaterials",
                    column: "CategoryId",
                    principalTable: "RawMaterialCategories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);

             
            }

            protected override void Down(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.DropForeignKey(
                    name: "FK_RawMaterials_RawMaterialCategories_CategoryId",
                    table: "RawMaterials");

                migrationBuilder.DropForeignKey(
                    name: "FK_RawMaterials_Users_supplierId",
                    table: "RawMaterials");

                migrationBuilder.DropForeignKey(
                    name: "FK_UserInteractions_RawMaterials_RawMaterialId",
                    table: "UserInteractions");

                migrationBuilder.DropIndex(
                    name: "IX_RawMaterials_CategoryId",
                    table: "RawMaterials");

                migrationBuilder.DropIndex(
                    name: "IX_RawMaterials_supplierId",
                    table: "RawMaterials");

                migrationBuilder.DropColumn(
                    name: "CategoryId",
                    table: "RawMaterials");

                migrationBuilder.DropColumn(
                    name: "supplierId",
                    table: "RawMaterials");

                migrationBuilder.DropPrimaryKey(
                    name: "PK_RawMaterials",
                    table: "RawMaterials");

                migrationBuilder.RenameTable(
                    name: "RawMaterials",
                    newName: "RawMaterial");

                migrationBuilder.AddPrimaryKey(
                    name: "PK_RawMaterial",
                    table: "RawMaterial",
                    column: "Id");

                migrationBuilder.AddColumn<bool>(
                    name: "IsFavourite",
                    table: "UserInteractions",
                    type: "bit",
                    nullable: true);

                migrationBuilder.AddForeignKey(
                    name: "FK_UserInteractions_RawMaterial_RawMaterialId",
                    table: "UserInteractions",
                    column: "RawMaterialId",
                    principalTable: "RawMaterial",
                    principalColumn: "Id");
            }
        }
}
