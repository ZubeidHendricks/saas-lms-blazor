using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SaasLMS.Server.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subdomain = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomDomain = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Plan = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ThemeConfig = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StorageQuota = table.Column<long>(type: "bigint", nullable: false),
                    StorageUsed = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Subdomain",
                table: "Tenants",
                column: "Subdomain",
                unique: true);

            // Add other tables and relationships
            // This is just the initial tenant table setup
            // Full migration would include all other tables
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}