using Microsoft.EntityFrameworkCore.Migrations;

namespace Junjuria.Infrastructure.Data.Migrations
{
    public partial class EdditedRecomendations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recomendations_AspNetUsers_UserId",
                table: "Recomendations");

            migrationBuilder.DropIndex(
                name: "IX_Recomendations_UserId",
                table: "Recomendations");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Recomendations",
                newName: "Author");

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Recomendations",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Recomendations",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Recomendations");

            migrationBuilder.RenameColumn(
                name: "Author",
                table: "Recomendations",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Recomendations",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recomendations_UserId",
                table: "Recomendations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recomendations_AspNetUsers_UserId",
                table: "Recomendations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
