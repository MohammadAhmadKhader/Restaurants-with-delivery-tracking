using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Migrations
{
    /// <inheritdoc />
    public partial class Fixing_RestaurantRoles_NormalizedName_Index_Uniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RestaurantRoles_NormalizedName",
                table: "RestaurantRoles");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantRoles_RestaurantId_NormalizedName",
                table: "RestaurantRoles",
                columns: new[] { "RestaurantId", "NormalizedName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RestaurantRoles_RestaurantId_NormalizedName",
                table: "RestaurantRoles");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantRoles_NormalizedName",
                table: "RestaurantRoles",
                column: "NormalizedName",
                unique: true);
        }
    }
}
