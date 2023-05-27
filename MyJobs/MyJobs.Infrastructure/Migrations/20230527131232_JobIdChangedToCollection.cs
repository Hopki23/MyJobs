using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyJobs.Infrastructure.Migrations
{
    public partial class JobIdChangedToCollection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "CVJob",
                columns: table => new
                {
                    JobsId = table.Column<int>(type: "int", nullable: false),
                    ResumesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVJob", x => new { x.JobsId, x.ResumesId });
                    table.ForeignKey(
                        name: "FK_CVJob_CVs_ResumesId",
                        column: x => x.ResumesId,
                        principalTable: "CVs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CVJob_Jobs_JobsId",
                        column: x => x.JobsId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CVJob_ResumesId",
                table: "CVJob",
                column: "ResumesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CVJob");

            migrationBuilder.AddColumn<int>(
                name: "JobId",
                table: "CVs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CVs_JobId",
                table: "CVs",
                column: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_CVs_Jobs_JobId",
                table: "CVs",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id");
        }
    }
}
