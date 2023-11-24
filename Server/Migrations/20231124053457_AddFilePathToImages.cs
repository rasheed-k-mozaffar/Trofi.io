using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Trofi.io.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddFilePathToImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Images",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "ADMIN", "ADMIN" },
                    { "2", null, "USER", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "CartId", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "Location", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "b5fe9066-a579-407e-b90f-bc86cb6348f5", 0, new Guid("00000000-0000-0000-0000-000000000000"), "934c36d1-2eac-4edd-87a3-28c22cbeb7cd", "admin@admin.com", false, "Admin", "Admin", "Main", false, null, "ADMIN@ADMIN.COM", "ADMIN", "AQAAAAIAAYagAAAAENQCrPojXaNBvlcPvmj1fVlYylCrgM+OG5fWh8SbACoyFxPMnpJ7Fim3xDz51zKJRA==", "0000", false, "c4dd2ecf-a6e5-4d51-9125-d413d5203f15", false, "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "b5fe9066-a579-407e-b90f-bc86cb6348f5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "b5fe9066-a579-407e-b90f-bc86cb6348f5" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b5fe9066-a579-407e-b90f-bc86cb6348f5");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Images");
        }
    }
}
