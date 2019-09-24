using Microsoft.EntityFrameworkCore.Migrations;

namespace Junjuria.Infrastructure.Data.Migrations
{
    public partial class DiscountSetToDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductComments_Products_ProductId",
                table: "ProductComments");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Products",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductComments",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductComments_Products_ProductId",
                table: "ProductComments",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductComments_Products_ProductId",
                table: "ProductComments");

            migrationBuilder.AlterColumn<double>(
                name: "Discount",
                table: "Products",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductComments",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ProductComments_Products_ProductId",
                table: "ProductComments",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
