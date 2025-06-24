using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class BackgroundImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4010c7d8-4cc4-43d2-8275-bac97484f762");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "924622f4-deb9-46c6-b9ea-ba6313e24da3");

            migrationBuilder.AddColumn<string>(
                name: "BackgroundImage",
                table: "Movie",
                type: "VARCHAR(500)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "540c894a-490b-4f82-90a8-b892f3cc3533", null, "User", "USER" },
                    { "5ef45dc2-5061-4662-8364-03e9356aef25", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "540c894a-490b-4f82-90a8-b892f3cc3533");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ef45dc2-5061-4662-8364-03e9356aef25");

            migrationBuilder.DropColumn(
                name: "BackgroundImage",
                table: "Movie");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4010c7d8-4cc4-43d2-8275-bac97484f762", null, "Admin", "ADMIN" },
                    { "924622f4-deb9-46c6-b9ea-ba6313e24da3", null, "User", "USER" }
                });
        }
    }
}
