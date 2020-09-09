using Microsoft.EntityFrameworkCore.Migrations;

namespace Financier.Common.Migrations
{
    public partial class Add_AccountName_To_Card : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "Expenses_Cards",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "Expenses_Cards");
        }
    }
}
