using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ship_convenient.Migrations
{
    public partial class intidb2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InfoUser_Phone",
                table: "InfoUser");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "TransactionPackage",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PackageId",
                table: "Notification",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "InfoUser",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_PackageId",
                table: "Notification",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Package_PackageId",
                table: "Notification",
                column: "PackageId",
                principalTable: "Package",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Package_PackageId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_PackageId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "TransactionPackage");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "Notification");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "InfoUser",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_InfoUser_Phone",
                table: "InfoUser",
                column: "Phone",
                unique: true);
        }
    }
}
