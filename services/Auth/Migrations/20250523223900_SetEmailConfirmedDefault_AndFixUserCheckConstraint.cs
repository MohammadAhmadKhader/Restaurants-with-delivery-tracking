using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Migrations
{
    /// <inheritdoc />
    public partial class SetEmailConfirmedDefault_AndFixUserCheckConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_DeletedOrRequiredFields",
                table: "Users");

            migrationBuilder.AlterColumn<bool>(
                name: "EmailConfirmed",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_DeletedOrRequiredFields",
                table: "Users",
                sql: "\r\n                    \"IsDeleted\" = FALSE OR (\r\n                        \"Email\"               IS NULL AND\r\n                        \"UserName\"            IS NULL AND\r\n                        \"NormalizedUserName\"  IS NULL AND\r\n                        \"NormalizedEmail\"     IS NULL AND\r\n                        \"PasswordHash\"        IS NULL AND\r\n                        \"FirstName\"           IS NULL AND\r\n                        \"LastName\"            IS NULL\r\n                    )");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_DeletedOrRequiredFields",
                table: "Users");

            migrationBuilder.AlterColumn<bool>(
                name: "EmailConfirmed",
                table: "Users",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_DeletedOrRequiredFields",
                table: "Users",
                sql: "\r\n                    \"IsDeleted\" = TRUE OR (\r\n                        \"Email\" IS NOT NULL AND\r\n                        \"PasswordHash\" IS NOT NULL AND\r\n                        \"FirstName\" IS NOT NULL AND\r\n                        \"LastName\" IS NOT NULL\r\n                    )");
        }
    }
}
