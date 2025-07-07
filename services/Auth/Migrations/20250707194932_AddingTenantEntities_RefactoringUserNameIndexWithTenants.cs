using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Auth.Migrations
{
    /// <inheritdoc />
    public partial class AddingTenantEntities_RefactoringUserNameIndexWithTenants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "IsGlobal",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RestaurantPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    IsDefaultUser = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefaultAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefaultOwner = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantViewDto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    IsOpen = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantViewDto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantRoles_RestaurantViewDto_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "RestaurantViewDto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantPermissionRestaurantRole",
                columns: table => new
                {
                    PermissionsId = table.Column<int>(type: "integer", nullable: false),
                    RolesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantPermissionRestaurantRole", x => new { x.PermissionsId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_RestaurantPermissionRestaurantRole_RestaurantPermissions_Pe~",
                        column: x => x.PermissionsId,
                        principalTable: "RestaurantPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestaurantPermissionRestaurantRole_RestaurantRoles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "RestaurantRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRestaurantRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRestaurantRoles", x => new { x.UserId, x.RestaurantId });
                    table.ForeignKey(
                        name: "FK_UserRestaurantRoles_RestaurantRoles_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "RestaurantRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRestaurantRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RestaurantId_NormalizedEmail",
                table: "Users",
                columns: new[] { "RestaurantId", "NormalizedEmail" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RestaurantId_NormalizedUserName",
                table: "Users",
                columns: new[] { "RestaurantId", "NormalizedUserName" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_IsGlobalOrRestaurantNull",
                table: "Users",
                sql: " \r\n                    \"IsGlobal\" = TRUE OR (\r\n                        \"RestaurantId\"  IS NULL\r\n                    )");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantPermissionRestaurantRole_RolesId",
                table: "RestaurantPermissionRestaurantRole",
                column: "RolesId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantPermissions_NormalizedName",
                table: "RestaurantPermissions",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantRoles_NormalizedName",
                table: "RestaurantRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantRoles_RestaurantId",
                table: "RestaurantRoles",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRestaurantRoles_RestaurantId",
                table: "UserRestaurantRoles",
                column: "RestaurantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestaurantPermissionRestaurantRole");

            migrationBuilder.DropTable(
                name: "UserRestaurantRoles");

            migrationBuilder.DropTable(
                name: "RestaurantPermissions");

            migrationBuilder.DropTable(
                name: "RestaurantRoles");

            migrationBuilder.DropTable(
                name: "RestaurantViewDto");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RestaurantId_NormalizedEmail",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RestaurantId_NormalizedUserName",
                table: "Users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_IsGlobalOrRestaurantNull",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsGlobal",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);
        }
    }
}
