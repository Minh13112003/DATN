using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class IdentityRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ed666c5-637d-41f8-a7fe-e65130e8c622");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fd94bd31-e933-4f55-a9c4-885d178e6de3");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5046c082-a101-4e37-9b52-a259f87d49d1", null, "Admin", "ADMIN" },
                    { "d3f5d680-99ad-4b4d-83e9-50edd10bcbb2", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { "5ed666c5-637d-41f8-a7fe-e65130e8c622", null, "User", "USER" },
                    { "fd94bd31-e933-4f55-a9c4-885d178e6de3", null, "Admin", "ADMIN" }
                });
        }
    }
}
