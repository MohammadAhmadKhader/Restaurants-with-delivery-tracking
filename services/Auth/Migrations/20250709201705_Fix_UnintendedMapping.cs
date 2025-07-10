using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Migrations
{
    /// <inheritdoc />
    public partial class Fix_UnintendedMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantRoles_RestaurantViewDto_RestaurantId",
                table: "RestaurantRoles");

            migrationBuilder.DropTable(
                name: "RestaurantViewDto");

            migrationBuilder.DropIndex(
                name: "IX_RestaurantRoles_RestaurantId",
                table: "RestaurantRoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestaurantViewDto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsOpen = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantViewDto", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantRoles_RestaurantId",
                table: "RestaurantRoles",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantRoles_RestaurantViewDto_RestaurantId",
                table: "RestaurantRoles",
                column: "RestaurantId",
                principalTable: "RestaurantViewDto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
