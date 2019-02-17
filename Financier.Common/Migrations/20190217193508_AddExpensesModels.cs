using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Financier.Common.Migrations
{
    public partial class AddExpensesModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Expenses_Cards",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Number = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses_Cards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Expenses_Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Expenses_Statements",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CardId = table.Column<Guid>(nullable: false),
                    PostedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses_Statements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_Statements_Expenses_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Expenses_Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Expenses_Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    StatementId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    TransactedAt = table.Column<DateTime>(nullable: false),
                    PostedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_Items_Expenses_Statements_StatementId",
                        column: x => x.StatementId,
                        principalTable: "Expenses_Statements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemTags",
                columns: table => new
                {
                    ItemId = table.Column<Guid>(nullable: false),
                    TagId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTags", x => new { x.ItemId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ItemTags_Expenses_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Expenses_Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemTags_Expenses_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Expenses_Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Items_StatementId",
                table: "Expenses_Items",
                column: "StatementId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Statements_CardId",
                table: "Expenses_Statements",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Tags_Name",
                table: "Expenses_Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemTags_TagId",
                table: "ItemTags",
                column: "TagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemTags");

            migrationBuilder.DropTable(
                name: "Expenses_Items");

            migrationBuilder.DropTable(
                name: "Expenses_Tags");

            migrationBuilder.DropTable(
                name: "Expenses_Statements");

            migrationBuilder.DropTable(
                name: "Expenses_Cards");
        }
    }
}
