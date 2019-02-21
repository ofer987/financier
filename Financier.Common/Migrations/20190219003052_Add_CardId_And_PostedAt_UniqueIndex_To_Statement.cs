using Microsoft.EntityFrameworkCore.Migrations;

namespace Financier.Common.Migrations
{
    public partial class Add_CardId_And_PostedAt_UniqueIndex_To_Statement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Expenses_Statements_CardId",
                table: "Expenses_Statements");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Statements_CardId_PostedAt",
                table: "Expenses_Statements",
                columns: new[] { "CardId", "PostedAt" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Expenses_Statements_CardId_PostedAt",
                table: "Expenses_Statements");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Statements_CardId",
                table: "Expenses_Statements",
                column: "CardId");
        }
    }
}
