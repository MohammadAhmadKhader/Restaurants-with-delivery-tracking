using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Migrations
{
    /// <inheritdoc />
    public partial class FixingUserRestaurantRoleRoldIdColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRestaurantRoles_RestaurantRoles_RestaurantId",
                table: "UserRestaurantRoles");

            migrationBuilder.RenameColumn(
                name: "RestaurantId",
                table: "UserRestaurantRoles",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRestaurantRoles_RestaurantId",
                table: "UserRestaurantRoles",
                newName: "IX_UserRestaurantRoles_RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRestaurantRoles_RestaurantRoles_RoleId",
                table: "UserRestaurantRoles",
                column: "RoleId",
                principalTable: "RestaurantRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRestaurantRoles_RestaurantRoles_RoleId",
                table: "UserRestaurantRoles");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "UserRestaurantRoles",
                newName: "RestaurantId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRestaurantRoles_RoleId",
                table: "UserRestaurantRoles",
                newName: "IX_UserRestaurantRoles_RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRestaurantRoles_RestaurantRoles_RestaurantId",
                table: "UserRestaurantRoles",
                column: "RestaurantId",
                principalTable: "RestaurantRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
