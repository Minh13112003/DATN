using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class Slug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7ff36ad0-f2f6-40ad-8a42-c0829a35e68e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f3ffdc0e-022d-4785-8378-8b9f86205054");

            migrationBuilder.AddColumn<string>(
                name: "SlugCatelogies",
                table: "Movie",
                type: "VARCHAR(255)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SlugTitle",
                table: "Movie",
                type: "VARCHAR(255)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0fffbd72-8e33-49c3-a540-2d5d6469eea6", null, "Admin", "ADMIN" },
                    { "5f659606-a41f-43e8-8241-d80429fbabf2", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0fffbd72-8e33-49c3-a540-2d5d6469eea6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f659606-a41f-43e8-8241-d80429fbabf2");

            migrationBuilder.DropColumn(
                name: "SlugCatelogies",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "SlugTitle",
                table: "Movie");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7ff36ad0-f2f6-40ad-8a42-c0829a35e68e", null, "User", "USER" },
                    { "f3ffdc0e-022d-4785-8378-8b9f86205054", null, "Admin", "ADMIN" }
                });
        }
    }
}
