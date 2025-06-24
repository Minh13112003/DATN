using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class AddImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "735099ef-65b4-48e4-87c8-747f43ced3e2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cfe9fa0c-32a4-4d50-b62e-020c3dd424ce");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Movie",
                type: "VARCHAR(500)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7ff36ad0-f2f6-40ad-8a42-c0829a35e68e", null, "User", "USER" },
                    { "f3ffdc0e-022d-4785-8378-8b9f86205054", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7ff36ad0-f2f6-40ad-8a42-c0829a35e68e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f3ffdc0e-022d-4785-8378-8b9f86205054");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Movie");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "735099ef-65b4-48e4-87c8-747f43ced3e2", null, "Admin", "ADMIN" },
                    { "cfe9fa0c-32a4-4d50-b62e-020c3dd424ce", null, "User", "USER" }
                });
        }
    }
}
