using Microsoft.EntityFrameworkCore.Migrations;

namespace Financier.Common.Migrations
{
    public partial class Add_Account_To_Card_Via_Owner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "Expenses_Cards",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Cards_AccountName",
                table: "Expenses_Cards",
                column: "AccountName");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Cards_Expenses_Accounts_AccountName",
                table: "Expenses_Cards",
                column: "AccountName",
                principalTable: "Expenses_Accounts",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Cards_Expenses_Accounts_AccountName",
                table: "Expenses_Cards");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_Cards_AccountName",
                table: "Expenses_Cards");

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "Expenses_Cards",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
