using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class AutoDeleteSubCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0e0c2186-c6af-4c45-8be6-f70dc0accaed");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cbde32c8-3c27-490d-a916-581a8b173faf");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "70d81eb2-6c5a-4e25-8f98-6d0a8284bf8c", null, "User", "USER" },
                    { "90bb9e66-57e7-401f-a30c-2caa0f386029", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "70d81eb2-6c5a-4e25-8f98-6d0a8284bf8c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "90bb9e66-57e7-401f-a30c-2caa0f386029");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0e0c2186-c6af-4c45-8be6-f70dc0accaed", null, "User", "USER" },
                    { "cbde32c8-3c27-490d-a916-581a8b173faf", null, "Admin", "ADMIN" }
                });
        }
    }
}
