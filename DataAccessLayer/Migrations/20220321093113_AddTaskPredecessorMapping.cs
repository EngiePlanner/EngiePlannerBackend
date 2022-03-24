using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddTaskPredecessorMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskPredecessorsMappings",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    PredecessorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskPredecessorsMappings", x => new { x.TaskId, x.PredecessorId });
                    table.ForeignKey(
                        name: "FK_TaskPredecessorsMappings_Tasks_PredecessorId",
                        column: x => x.PredecessorId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskPredecessorsMappings_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskPredecessorsMappings_PredecessorId",
                table: "TaskPredecessorsMappings",
                column: "PredecessorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskPredecessorsMappings");
        }
    }
}
