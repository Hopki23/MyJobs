using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyJobs.Infrastructure.Migrations
{
    public partial class AddedTwoNewPropertiesToJobsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Jobs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Town",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Town",
                table: "Jobs");
        }
    }
}
