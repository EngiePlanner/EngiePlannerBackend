using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class RemoveSuccessorColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Tasks_SuccessorId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_SuccessorId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SuccessorId",
                table: "Tasks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SuccessorId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_SuccessorId",
                table: "Tasks",
                column: "SuccessorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Tasks_SuccessorId",
                table: "Tasks",
                column: "SuccessorId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
