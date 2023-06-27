using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyJobs.Infrastructure.Migrations
{
    public partial class IsDeletedPropertyAddedToCategoryEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsDeleted", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "dea12856-c198-4129-b3f3-b893d8395082", 0, "485aac32-08df-4db6-9e8b-52f3845b7f1a", "admin@abv.bg", false, "Admin", false, "Admin", false, null, "ADMIN@ABV.BG", "ADMIN", "AQAAAAEAACcQAAAAEJoYXXa8BWRuT7Jl1Us+ljDTPdt1NamqQhYTcq6Qmg/HZffGjIl3gEjx0nxjFlF5Kw==", null, false, "7f81a7cd-7a4c-4f97-b5e8-3d67581ebf77", false, "admin" });
        }
    }
}
