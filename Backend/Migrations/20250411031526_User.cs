using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "12a39b47-ed61-4ab2-9716-c859cd41f60d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "51c47711-4215-4f49-9746-d3fee8b8239a");

            migrationBuilder.AddColumn<string>(
                name: "FavoriteSlugTitle",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVip",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2c77918b-e8d6-4c90-8315-62832f225cb5", null, "User", "USER" },
                    { "3de027c5-b687-4e85-b8d1-af25eaa3297a", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c77918b-e8d6-4c90-8315-62832f225cb5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3de027c5-b687-4e85-b8d1-af25eaa3297a");

            migrationBuilder.DropColumn(
                name: "FavoriteSlugTitle",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsVip",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "12a39b47-ed61-4ab2-9716-c859cd41f60d", null, "Admin", "ADMIN" },
                    { "51c47711-4215-4f49-9746-d3fee8b8239a", null, "User", "USER" }
                });
        }
    }
}
