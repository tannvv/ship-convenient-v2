using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ship_convenient.Migrations
{
    public partial class initdb6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepositId",
                table: "Transaction",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionIdPartner",
                table: "Deposit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_DepositId",
                table: "Transaction",
                column: "DepositId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Deposit_DepositId",
                table: "Transaction",
                column: "DepositId",
                principalTable: "Deposit",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Deposit_DepositId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_DepositId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "DepositId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "TransactionIdPartner",
                table: "Deposit");
        }
    }
}
