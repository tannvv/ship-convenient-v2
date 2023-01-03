using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ship_convenient.Migrations
{
    public partial class initdb3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InfoUser_Account_AccountId",
                table: "InfoUser");

            migrationBuilder.DropIndex(
                name: "IX_InfoUser_AccountId",
                table: "InfoUser");

            migrationBuilder.CreateIndex(
                name: "IX_Account_InfoUserId",
                table: "Account",
                column: "InfoUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Account_InfoUser_InfoUserId",
                table: "Account",
                column: "InfoUserId",
                principalTable: "InfoUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_InfoUser_InfoUserId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_InfoUserId",
                table: "Account");

            migrationBuilder.CreateIndex(
                name: "IX_InfoUser_AccountId",
                table: "InfoUser",
                column: "AccountId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InfoUser_Account_AccountId",
                table: "InfoUser",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
