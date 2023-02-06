using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ship_convenient.Migrations
{
    public partial class initdb10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegistrationToken",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationToken",
                table: "Account");
        }
    }
}
