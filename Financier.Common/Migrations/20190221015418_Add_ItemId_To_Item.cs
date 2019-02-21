using Microsoft.EntityFrameworkCore.Migrations;

namespace Financier.Common.Migrations
{
    public partial class Add_ItemId_To_Item : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Expenses_Items_StatementId",
                table: "Expenses_Items");

            migrationBuilder.AddColumn<string>(
                name: "ItemId",
                table: "Expenses_Items",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Items_StatementId_ItemId",
                table: "Expenses_Items",
                columns: new[] { "StatementId", "ItemId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Expenses_Items_StatementId_ItemId",
                table: "Expenses_Items");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Expenses_Items");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Items_StatementId",
                table: "Expenses_Items",
                column: "StatementId");
        }
    }
}
