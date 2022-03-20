using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class AddedPredecessors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Delivery_DeliveryId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "Delivery");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_DeliveryId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DeliveryId",
                table: "Tasks");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "DeliveryId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Delivery",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Delivery", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DeliveryId",
                table: "Tasks",
                column: "DeliveryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Delivery_DeliveryId",
                table: "Tasks",
                column: "DeliveryId",
                principalTable: "Delivery",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
