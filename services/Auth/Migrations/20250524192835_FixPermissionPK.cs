using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Migrations
{
    /// <inheritdoc />
    public partial class FixPermissionPK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRole_Permissions_PermissionsMyProperty",
                table: "PermissionRole");

            migrationBuilder.RenameColumn(
                name: "MyProperty",
                table: "Permissions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PermissionsMyProperty",
                table: "PermissionRole",
                newName: "PermissionsId");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRole_Permissions_PermissionsId",
                table: "PermissionRole",
                column: "PermissionsId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRole_Permissions_PermissionsId",
                table: "PermissionRole");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Permissions",
                newName: "MyProperty");

            migrationBuilder.RenameColumn(
                name: "PermissionsId",
                table: "PermissionRole",
                newName: "PermissionsMyProperty");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRole_Permissions_PermissionsMyProperty",
                table: "PermissionRole",
                column: "PermissionsMyProperty",
                principalTable: "Permissions",
                principalColumn: "MyProperty",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
