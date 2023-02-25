using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ship_convenient.Migrations
{
    public partial class initdb17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Volume",
                table: "Package",
                newName: "Width");

            migrationBuilder.AddColumn<double>(
                name: "Height",
                table: "Package",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Length",
                table: "Package",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "Package");

            migrationBuilder.RenameColumn(
                name: "Width",
                table: "Package",
                newName: "Volume");
        }
    }
}
