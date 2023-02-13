using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ship_convenient.Migrations
{
    public partial class initdb18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypeOfFeedback",
                table: "Feedback",
                newName: "FeedbackFor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FeedbackFor",
                table: "Feedback",
                newName: "TypeOfFeedback");
        }
    }
}
