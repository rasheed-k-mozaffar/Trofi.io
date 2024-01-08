using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trofi.io.Server.Migrations
{
    /// <inheritdoc />
    public partial class ModifyReviewsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "CustomerReviews",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedOn",
                table: "CustomerReviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewerId",
                table: "CustomerReviews",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WrittenOn",
                table: "CustomerReviews",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerReviews_AspNetUsers_ReviewerId",
                table: "CustomerReviews");

            migrationBuilder.DropIndex(
                name: "IX_CustomerReviews_ReviewerId",
                table: "CustomerReviews");

            migrationBuilder.DropColumn(
                name: "EditedOn",
                table: "CustomerReviews");

            migrationBuilder.DropColumn(
                name: "ReviewerId",
                table: "CustomerReviews");

            migrationBuilder.DropColumn(
                name: "WrittenOn",
                table: "CustomerReviews");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "CustomerReviews",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
