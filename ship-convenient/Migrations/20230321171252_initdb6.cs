using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ship_convenient.Migrations
{
    public partial class initdb6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Distance",
                table: "Route",
                newName: "DistanceForward");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Report",
                newName: "Reason");

            migrationBuilder.AddColumn<double>(
                name: "DistanceBackward",
                table: "Route",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistanceBackward",
                table: "Route");

            migrationBuilder.RenameColumn(
                name: "DistanceForward",
                table: "Route",
                newName: "Distance");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "Report",
                newName: "Content");
        }
    }
}
