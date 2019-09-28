using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Junjuria.Infrastructure.Data.Migrations
{
    public partial class RemovedPropertiesFromUserFavouriteProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfCreation",
                table: "UserFavouriteProduct");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserFavouriteProduct");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfCreation",
                table: "UserFavouriteProduct",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserFavouriteProduct",
                nullable: false,
                defaultValue: false);
        }
    }
}
