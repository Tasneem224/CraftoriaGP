using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailOTPIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_emailVerificationCodes",
                table: "emailVerificationCodes");

            migrationBuilder.RenameTable(
                name: "emailVerificationCodes",
                newName: "EmailVerificationCodes");

            migrationBuilder.AddColumn<bool>(
                name: "IsOld",
                table: "EmailVerificationCodes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailVerificationCodes",
                table: "EmailVerificationCodes",
                column: "Id");
            migrationBuilder.CreateIndex(
                name: "IX_EmailVerificationCodes_Email_IsUsed_ExpirationTime",
                table: "EmailVerificationCodes",
                columns: new[] { "Email", "IsUsed", "ExpirationTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailVerificationCodes",
                table: "EmailVerificationCodes");

            migrationBuilder.DropColumn(
                name: "IsOld",
                table: "EmailVerificationCodes");

            migrationBuilder.RenameTable(
                name: "EmailVerificationCodes",
                newName: "emailVerificationCodes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_emailVerificationCodes",
                table: "emailVerificationCodes",
                column: "Id");
            migrationBuilder.DropIndex(
                name: "IX_EmailVerificationCodes_Email_IsUsed_ExpirationTime",
                table: "EmailVerificationCodes");

        }
    }
}
