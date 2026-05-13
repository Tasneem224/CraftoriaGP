using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddIsVisibleToMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // هنضيف العمود يدوياً لجدول Messages
            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: true); // خليه true عشان الرسايل القديمة متختفيش

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // كود المسح في حالة الـ Rollback
            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "Messages");
        }
    }
}
