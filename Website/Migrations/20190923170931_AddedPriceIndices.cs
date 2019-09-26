using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Migrations
{
    public partial class AddedPriceIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductContent",
                table: "ProductContent");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "ProductContent",
                unicode: false,
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductContent",
                table: "ProductContent",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PriceIndices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductContentId = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    Index = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceIndices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceIndices_ProductContent",
                        column: x => x.ProductContentId,
                        principalTable: "ProductContent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductContent_ProductId",
                table: "ProductContent",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceIndices_ProductContentId",
                table: "PriceIndices",
                column: "ProductContentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceIndices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductContent",
                table: "ProductContent");

            migrationBuilder.DropIndex(
                name: "IX_ProductContent_ProductId",
                table: "ProductContent");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductContent");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductContent",
                table: "ProductContent",
                columns: new[] { "ProductId", "ProductContentTypeId" });
        }
    }
}
