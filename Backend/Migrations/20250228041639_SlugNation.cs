using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class SlugNation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7370e776-0bfe-4b75-9361-9b4b1d399bea");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c0a978ba-68f7-41b1-bf64-3195e2181530");

            migrationBuilder.AddColumn<string>(
                name: "SlugNation",
                table: "Movie",
                type: "VARCHAR(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "515228c8-189d-482b-8e96-7b1851d33342", null, "Admin", "ADMIN" },
                    { "604bda3d-3da5-49e8-8756-c0d2c750229c", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "515228c8-189d-482b-8e96-7b1851d33342");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "604bda3d-3da5-49e8-8756-c0d2c750229c");

            migrationBuilder.DropColumn(
                name: "SlugNation",
                table: "Movie");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7370e776-0bfe-4b75-9361-9b4b1d399bea", null, "Admin", "ADMIN" },
                    { "c0a978ba-68f7-41b1-bf64-3195e2181530", null, "User", "USER" }
                });
        }
    }
}
