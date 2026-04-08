using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class categoryIDColumnToRawMateial : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. فك الارتباط القديم مع UserInteractions
            migrationBuilder.DropForeignKey(
                name: "FK_UserInteractions_RawMaterial_RawMaterialId",
                table: "UserInteractions");

            // 2. مسح عمود IsFavourite من UserInteractions
            migrationBuilder.DropColumn(
                name: "IsFavourite",
                table: "UserInteractions");

            // 3. مسح الـ Primary Key القديم
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

            // 🚨 ملحوظة: شيلنا أوامر إضافة أعمدة CategoryId و supplierId 
            // والـ Indexes بتاعتهم لأنهم موجودين بالفعل على الريموت سيرفر 🚨

            // 6. ربط العلاقات بالاسم الجديد للجدول
            migrationBuilder.AddForeignKey(
                name: "FK_UserInteractions_RawMaterials_RawMaterialId",
                table: "UserInteractions",
                column: "RawMaterialId",
                principalTable: "RawMaterials",
                principalColumn: "Id");


            // 7. التأكيد على روابط الـ Category والمورد
            migrationBuilder.AddForeignKey(
                name: "FK_RawMaterials_RawMaterialCategories_CategoryId",
                table: "RawMaterials",
                column: "CategoryId",
                principalTable: "RawMaterialCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_RawMaterials_Users_supplierId",
                table: "RawMaterials",
                column: "supplierId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // التنظيف العكسي (لو حبيتي تتراجعي عن الميجريشن)

            migrationBuilder.DropForeignKey(
                name: "FK_UserInteractions_RawMaterials_RawMaterialId",
                table: "UserInteractions");

            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterials_RawMaterialCategories_CategoryId",
                table: "RawMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_RawMaterials_Users_supplierId",
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
