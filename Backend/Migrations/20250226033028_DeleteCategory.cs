using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class DeleteCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0f78deae-16de-4342-ba2d-2ab975ee664b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "279e7f90-9bca-4882-9522-dfdaf5a9e5af");

            migrationBuilder.DropColumn(
                name: "Catelogies",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "SlugCatelogies",
                table: "Movie");

            migrationBuilder.AddColumn<string>(
                name: "SlugNameCategories",
                table: "Category",
                type: "VARCHAR(200)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "36b1dc56-2a57-4fa0-8cf0-b7e5dc820b61", null, "User", "USER" },
                    { "50d77c54-a902-4096-8faf-fbc7c0d46ca2", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "36b1dc56-2a57-4fa0-8cf0-b7e5dc820b61");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "50d77c54-a902-4096-8faf-fbc7c0d46ca2");

            migrationBuilder.DropColumn(
                name: "SlugNameCategories",
                table: "Category");

            migrationBuilder.AddColumn<string>(
                name: "Catelogies",
                table: "Movie",
                type: "VARCHAR(255)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SlugCatelogies",
                table: "Movie",
                type: "VARCHAR(255)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0f78deae-16de-4342-ba2d-2ab975ee664b", null, "Admin", "ADMIN" },
                    { "279e7f90-9bca-4882-9522-dfdaf5a9e5af", null, "User", "USER" }
                });
        }
    }
}
