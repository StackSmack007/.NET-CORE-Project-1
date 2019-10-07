using Microsoft.EntityFrameworkCore.Migrations;

namespace Junjuria.Infrastructure.Data.Migrations
{
    public partial class EFCore3_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Recomendations",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Recomendations",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
