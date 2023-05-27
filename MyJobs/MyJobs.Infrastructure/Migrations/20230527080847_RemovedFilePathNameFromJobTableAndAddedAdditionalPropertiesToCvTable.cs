using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyJobs.Infrastructure.Migrations
{
    public partial class RemovedFilePathNameFromJobTableAndAddedAdditionalPropertiesToCvTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResumeFilePath",
                table: "Jobs");

            migrationBuilder.AddColumn<int>(
                name: "JobId",
                table: "CVs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "ResumeFile",
                table: "CVs",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "ResumeFileName",
                table: "CVs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CVs_JobId",
                table: "CVs",
                column: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_CVs_Jobs_JobId",
                table: "CVs",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CVs_Jobs_JobId",
                table: "CVs");

            migrationBuilder.DropIndex(
                name: "IX_CVs_JobId",
                table: "CVs");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "CVs");

            migrationBuilder.DropColumn(
                name: "ResumeFile",
                table: "CVs");

            migrationBuilder.DropColumn(
                name: "ResumeFileName",
                table: "CVs");

            migrationBuilder.AddColumn<string>(
                name: "ResumeFilePath",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
