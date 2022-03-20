using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class RemovePredecessors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Tasks_TaskEntityId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TaskEntityId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TaskEntityId",
                table: "Tasks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaskEntityId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskEntityId",
                table: "Tasks",
                column: "TaskEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Tasks_TaskEntityId",
                table: "Tasks",
                column: "TaskEntityId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
