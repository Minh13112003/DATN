using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class comment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "540c894a-490b-4f82-90a8-b892f3cc3533");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ef45dc2-5061-4662-8364-03e9356aef25");

            migrationBuilder.AddColumn<string>(
                name: "TimeComment",
                table: "Comment",
                type: "VARCHAR(150)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "498f5fc6-765f-449a-8da0-bd074471ef5d", null, "Admin", "ADMIN" },
                    { "84b656ad-4d6f-40ac-9b86-6764a98f670f", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "498f5fc6-765f-449a-8da0-bd074471ef5d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84b656ad-4d6f-40ac-9b86-6764a98f670f");

            migrationBuilder.DropColumn(
                name: "TimeComment",
                table: "Comment");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "540c894a-490b-4f82-90a8-b892f3cc3533", null, "User", "USER" },
                    { "5ef45dc2-5061-4662-8364-03e9356aef25", null, "Admin", "ADMIN" }
                });
        }
    }
}
