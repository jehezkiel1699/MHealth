using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MHealth.Migrations
{
    /// <inheritdoc />
    public partial class MRIPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MRIPosts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StaffId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PostDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRIPosts", x => new { x.Id, x.UserId, x.StaffId });
                    table.ForeignKey(
                        name: "FK_MRIPosts_Bookings_BookingId_UserId_StaffId",
                        columns: x => new { x.BookingId, x.UserId, x.StaffId },
                        principalTable: "Bookings",
                        principalColumns: new[] { "Id", "UserId", "StaffId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MRIPosts_BookingId_UserId_StaffId",
                table: "MRIPosts",
                columns: new[] { "BookingId", "UserId", "StaffId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MRIPosts");
        }
    }
}
