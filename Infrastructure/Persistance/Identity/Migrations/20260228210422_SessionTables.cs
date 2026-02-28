using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class SessionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpertAvailabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpertId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpertAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpertAvailabilities_Users_ExpertId",
                        column: x => x.ExpertId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExpertServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpertId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DurationInMinutes = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpertServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpertServices_Users_ExpertId",
                        column: x => x.ExpertId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BeginnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpertId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpertServiceId = table.Column<int>(type: "int", nullable: false),
                    ExpertAvailabilityId = table.Column<int>(type: "int", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    MeetingLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_ExpertAvailabilities_ExpertAvailabilityId",
                        column: x => x.ExpertAvailabilityId,
                        principalTable: "ExpertAvailabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessions_ExpertServices_ExpertServiceId",
                        column: x => x.ExpertServiceId,
                        principalTable: "ExpertServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_BeginnerId",
                        column: x => x.BeginnerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sessions_Users_ExpertId",
                        column: x => x.ExpertId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpertAvailabilities_ExpertId",
                table: "ExpertAvailabilities",
                column: "ExpertId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpertServices_ExpertId",
                table: "ExpertServices",
                column: "ExpertId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_BeginnerId",
                table: "Sessions",
                column: "BeginnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ExpertAvailabilityId",
                table: "Sessions",
                column: "ExpertAvailabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ExpertId",
                table: "Sessions",
                column: "ExpertId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ExpertServiceId",
                table: "Sessions",
                column: "ExpertServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "ExpertAvailabilities");

            migrationBuilder.DropTable(
                name: "ExpertServices");
        }
    }
}
