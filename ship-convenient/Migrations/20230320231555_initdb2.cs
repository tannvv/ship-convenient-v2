using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ship_convenient.Migrations
{
    public partial class initdb2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Report",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Report_AccountId",
                table: "Report",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Account_AccountId",
                table: "Report",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_Account_AccountId",
                table: "Report");

            migrationBuilder.DropIndex(
                name: "IX_Report_AccountId",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Report");
        }
    }
}
