using Microsoft.EntityFrameworkCore.Migrations;

namespace Junjuria.Infrastructure.Data.Migrations
{
    public partial class PictureDescriptionAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PictureDescription",
                table: "ProductPictures",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureDescription",
                table: "ProductPictures");
        }
    }
}
