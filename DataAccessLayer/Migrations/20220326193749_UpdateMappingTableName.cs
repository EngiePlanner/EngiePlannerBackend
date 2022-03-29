using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class UpdateMappingTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTaskMapping_Tasks_TaskId",
                table: "UserTaskMapping");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTaskMapping_Users_UserUsername",
                table: "UserTaskMapping");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTaskMapping",
                table: "UserTaskMapping");

            migrationBuilder.RenameTable(
                name: "UserTaskMapping",
                newName: "UserTaskMappings");

            migrationBuilder.RenameIndex(
                name: "IX_UserTaskMapping_TaskId",
                table: "UserTaskMappings",
                newName: "IX_UserTaskMappings_TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTaskMappings",
                table: "UserTaskMappings",
                columns: new[] { "UserUsername", "TaskId", "UserType" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserTaskMappings_Tasks_TaskId",
                table: "UserTaskMappings",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTaskMappings_Users_UserUsername",
                table: "UserTaskMappings",
                column: "UserUsername",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTaskMappings_Tasks_TaskId",
                table: "UserTaskMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTaskMappings_Users_UserUsername",
                table: "UserTaskMappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTaskMappings",
                table: "UserTaskMappings");

            migrationBuilder.RenameTable(
                name: "UserTaskMappings",
                newName: "UserTaskMapping");

            migrationBuilder.RenameIndex(
                name: "IX_UserTaskMappings_TaskId",
                table: "UserTaskMapping",
                newName: "IX_UserTaskMapping_TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTaskMapping",
                table: "UserTaskMapping",
                columns: new[] { "UserUsername", "TaskId", "UserType" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserTaskMapping_Tasks_TaskId",
                table: "UserTaskMapping",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTaskMapping_Users_UserUsername",
                table: "UserTaskMapping",
                column: "UserUsername",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
