using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Migrations
{
    public partial class UpdatedProductReviews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "ProductReviews");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "ProductReviews",
                unicode: false,
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReviewName",
                table: "Customers",
                unicode: false,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_CustomerId",
                table: "ProductReviews",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReviews_Customers",
                table: "ProductReviews",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductReviews_Customers",
                table: "ProductReviews");

            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_CustomerId",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "ReviewName",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "ProductReviews",
                unicode: false,
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
