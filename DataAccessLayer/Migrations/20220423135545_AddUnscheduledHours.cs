using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddUnscheduledHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvailableHours",
                table: "Availabilities",
                newName: "UnscheduledHours");

            migrationBuilder.AddColumn<int>(
                name: "DefaultAvailableHours",
                table: "Availabilities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultAvailableHours",
                table: "Availabilities");

            migrationBuilder.RenameColumn(
                name: "UnscheduledHours",
                table: "Availabilities",
                newName: "AvailableHours");
        }
    }
}
