using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Migrations
{
    /// <inheritdoc />
    public partial class Fixing_IsGlobalOrRestaurant_Constraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_IsGlobalOrRestaurantNull",
                table: "Users");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_IsGlobalOrRestaurantNull",
                table: "Users",
                sql: " \r\n                    \"IsGlobal\" = FALSE OR (\r\n                        \"RestaurantId\"  IS NULL\r\n                    )");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_IsGlobalOrRestaurantNull",
                table: "Users");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_IsGlobalOrRestaurantNull",
                table: "Users",
                sql: " \r\n                    \"IsGlobal\" = TRUE OR (\r\n                        \"RestaurantId\"  IS NULL\r\n                    )");
        }
    }
}
