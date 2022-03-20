using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class UpdateTaskEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTaskMappings");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeUsername",
                table: "Tasks",
                type: "nvarchar(10)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_EmployeeUsername",
                table: "Tasks",
                column: "EmployeeUsername");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_EmployeeUsername",
                table: "Tasks",
                column: "EmployeeUsername",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_EmployeeUsername",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_EmployeeUsername",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "EmployeeUsername",
                table: "Tasks");

            migrationBuilder.CreateTable(
                name: "UserTaskMappings",
                columns: table => new
                {
                    UserUsername = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTaskMappings", x => new { x.UserUsername, x.TaskId });
                    table.ForeignKey(
                        name: "FK_UserTaskMappings_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTaskMappings_Users_UserUsername",
                        column: x => x.UserUsername,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTaskMappings_TaskId",
                table: "UserTaskMappings",
                column: "TaskId");
        }
    }
}
