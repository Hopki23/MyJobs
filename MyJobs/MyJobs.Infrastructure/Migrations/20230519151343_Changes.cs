using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyJobs.Infrastructure.Migrations
{
    public partial class Changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Software Engineer" },
                    { 2, "Food and Hospitality" },
                    { 3, "Aviation and Aerospace" },
                    { 4, "Real Estate" },
                    { 5, "Education and Training" },
                    { 6, "Marketing and Advertising" },
                    { 7, "Healthcare and Medical" },
                    { 8, "Full-Stack Developer" },
                    { 9, "Back-End Developer" },
                    { 10, "Front-End Developer" },
                    { 11, "QA Tester" }
                });
        }
    }
}
