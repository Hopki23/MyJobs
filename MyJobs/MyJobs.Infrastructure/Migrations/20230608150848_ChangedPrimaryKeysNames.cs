using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyJobs.Infrastructure.Migrations
{
    public partial class ChangedPrimaryKeysNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeJob_Employees_EmployeesEmployeeId",
                table: "EmployeeJob");

            migrationBuilder.RenameColumn(
                name: "EmployerId",
                table: "Employers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Employees",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "EmployeesEmployeeId",
                table: "EmployeeJob",
                newName: "EmployeesId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeJob_Employees_EmployeesId",
                table: "EmployeeJob",
                column: "EmployeesId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeJob_Employees_EmployeesId",
                table: "EmployeeJob");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Employers",
                newName: "EmployerId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Employees",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "EmployeesId",
                table: "EmployeeJob",
                newName: "EmployeesEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeJob_Employees_EmployeesEmployeeId",
                table: "EmployeeJob",
                column: "EmployeesEmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
