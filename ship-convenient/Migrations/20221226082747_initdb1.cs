using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ship_convenient.Migrations
{
    public partial class initdb1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "InfoUser",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "InfoUser",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "InfoUser");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "InfoUser");
        }
    }
}
