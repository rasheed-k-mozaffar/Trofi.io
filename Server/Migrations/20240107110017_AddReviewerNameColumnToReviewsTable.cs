using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trofi.io.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewerNameColumnToReviewsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerReviews_AspNetUsers_ReviewerId",
                table: "CustomerReviews");

            migrationBuilder.DropIndex(
                name: "IX_CustomerReviews_ReviewerId",
                table: "CustomerReviews");

            migrationBuilder.DropColumn(
                name: "ReviewerId",
                table: "CustomerReviews");

            migrationBuilder.AddColumn<string>(
                name: "ReviwerName",
                table: "CustomerReviews",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviwerName",
                table: "CustomerReviews");

            migrationBuilder.AddColumn<string>(
                name: "ReviewerId",
                table: "CustomerReviews",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReviews_ReviewerId",
                table: "CustomerReviews",
                column: "ReviewerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerReviews_AspNetUsers_ReviewerId",
                table: "CustomerReviews",
                column: "ReviewerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
