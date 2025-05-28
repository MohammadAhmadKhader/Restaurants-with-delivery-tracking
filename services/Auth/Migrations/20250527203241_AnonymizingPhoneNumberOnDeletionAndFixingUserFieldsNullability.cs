using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Migrations
{
    /// <inheritdoc />
    public partial class AnonymizingPhoneNumberOnDeletionAndFixingUserFieldsNullability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_DeletedOrRequiredFields",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_DeletedOrRequiredFields",
                table: "Users",
                sql: "\r\n                    \"IsDeleted\" = FALSE OR (\r\n                        \"Email\"               IS NULL AND\r\n                        \"UserName\"            IS NULL AND\r\n                        \"NormalizedUserName\"  IS NULL AND\r\n                        \"NormalizedEmail\"     IS NULL AND\r\n                        \"PhoneNumber\"         IS NULL AND\r\n                        \"PasswordHash\"        IS NULL AND\r\n                        \"FirstName\"           IS NULL AND\r\n                        \"LastName\"            IS NULL\r\n                    )");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_DeletedOrRequiredFields",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_DeletedOrRequiredFields",
                table: "Users",
                sql: "\r\n                    \"IsDeleted\" = FALSE OR (\r\n                        \"Email\"               IS NULL AND\r\n                        \"UserName\"            IS NULL AND\r\n                        \"NormalizedUserName\"  IS NULL AND\r\n                        \"NormalizedEmail\"     IS NULL AND\r\n                        \"PasswordHash\"        IS NULL AND\r\n                        \"FirstName\"           IS NULL AND\r\n                        \"LastName\"            IS NULL\r\n                    )");
        }
    }
}
