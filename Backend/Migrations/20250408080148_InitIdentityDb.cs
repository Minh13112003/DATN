using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class InitIdentityDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5046c082-a101-4e37-9b52-a259f87d49d1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d3f5d680-99ad-4b4d-83e9-50edd10bcbb2");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "12a39b47-ed61-4ab2-9716-c859cd41f60d", null, "Admin", "ADMIN" },
                    { "51c47711-4215-4f49-9746-d3fee8b8239a", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "12a39b47-ed61-4ab2-9716-c859cd41f60d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "51c47711-4215-4f49-9746-d3fee8b8239a");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5046c082-a101-4e37-9b52-a259f87d49d1", null, "Admin", "ADMIN" },
                    { "d3f5d680-99ad-4b4d-83e9-50edd10bcbb2", null, "User", "USER" }
                });
        }
    }
}
