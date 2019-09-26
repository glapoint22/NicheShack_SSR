using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Migrations
{
    public partial class UpdatedProductContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductContent",
                table: "ProductContent");

            migrationBuilder.DropIndex(
                name: "IX_ProductContent_ProductId",
                table: "ProductContent");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "ProductPricePoints");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductContent");

            migrationBuilder.DropColumn(
                name: "PriceIndices",
                table: "ProductContent");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "ProductContent",
                newName: "ProductContentTypeId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProductPricePoints",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductContent",
                table: "ProductContent",
                columns: new[] { "ProductId", "ProductContentTypeId" });

            migrationBuilder.CreateTable(
                name: "ProductContentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(unicode: false, maxLength: 30, nullable: false),
                    Image = table.Column<string>(unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductContentTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductContent_ProductContentTypeId",
                table: "ProductContent",
                column: "ProductContentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductContent_ProductContentTypes",
                table: "ProductContent",
                column: "ProductContentTypeId",
                principalTable: "ProductContentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductContent_ProductContentTypes",
                table: "ProductContent");

            migrationBuilder.DropTable(
                name: "ProductContentTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductContent",
                table: "ProductContent");

            migrationBuilder.DropIndex(
                name: "IX_ProductContent_ProductContentTypeId",
                table: "ProductContent");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProductPricePoints");

            migrationBuilder.RenameColumn(
                name: "ProductContentTypeId",
                table: "ProductContent",
                newName: "Type");

            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "ProductPricePoints",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProductContent",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "PriceIndices",
                table: "ProductContent",
                unicode: false,
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductContent",
                table: "ProductContent",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductContent_ProductId",
                table: "ProductContent",
                column: "ProductId");
        }
    }
}
