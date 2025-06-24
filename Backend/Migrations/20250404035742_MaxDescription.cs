using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class MaxDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c5aa1141-642d-4455-83cd-c87dff42ae84");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fa0c4525-04c1-429a-9ad9-fdfb1c3a0d35");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Movie",
                type: "VARCHAR(1500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(500)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3f3a432e-16bb-4f03-9b83-f36f0bcf8201", null, "Admin", "ADMIN" },
                    { "6d8b0fd9-e709-4fe9-87ab-088a7619fde9", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3f3a432e-16bb-4f03-9b83-f36f0bcf8201");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6d8b0fd9-e709-4fe9-87ab-088a7619fde9");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Movie",
                type: "VARCHAR(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(1500)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "c5aa1141-642d-4455-83cd-c87dff42ae84", null, "User", "USER" },
                    { "fa0c4525-04c1-429a-9ad9-fdfb1c3a0d35", null, "Admin", "ADMIN" }
                });
        }
    }
}
