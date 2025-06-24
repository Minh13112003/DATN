using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class SlugType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "36b1dc56-2a57-4fa0-8cf0-b7e5dc820b61");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "50d77c54-a902-4096-8faf-fbc7c0d46ca2");

            migrationBuilder.AddColumn<string>(
                name: "SlugTypeMovie",
                table: "Movie",
                type: "VARCHAR(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0e0c2186-c6af-4c45-8be6-f70dc0accaed", null, "User", "USER" },
                    { "cbde32c8-3c27-490d-a916-581a8b173faf", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0e0c2186-c6af-4c45-8be6-f70dc0accaed");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cbde32c8-3c27-490d-a916-581a8b173faf");

            migrationBuilder.DropColumn(
                name: "SlugTypeMovie",
                table: "Movie");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "36b1dc56-2a57-4fa0-8cf0-b7e5dc820b61", null, "User", "USER" },
                    { "50d77c54-a902-4096-8faf-fbc7c0d46ca2", null, "Admin", "ADMIN" }
                });
        }
    }
}
