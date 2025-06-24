using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class List : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                    { "7370e776-0bfe-4b75-9361-9b4b1d399bea", null, "Admin", "ADMIN" },
                    { "c0a978ba-68f7-41b1-bf64-3195e2181530", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7370e776-0bfe-4b75-9361-9b4b1d399bea");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c0a978ba-68f7-41b1-bf64-3195e2181530");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "70d81eb2-6c5a-4e25-8f98-6d0a8284bf8c", null, "User", "USER" },
                    { "90bb9e66-57e7-401f-a30c-2caa0f386029", null, "Admin", "ADMIN" }
                });
        }
    }
}
