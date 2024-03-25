using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGardenWEB.Migrations
{
    /// <inheritdoc />
    public partial class New : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderDetailsId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderDetailsId",
                table: "Orders",
                column: "OrderDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderDetail_OrderDetailsId",
                table: "Orders",
                column: "OrderDetailsId",
                principalTable: "OrderDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderDetail_OrderDetailsId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderDetailsId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderDetailsId",
                table: "Orders");
        }
    }
}
