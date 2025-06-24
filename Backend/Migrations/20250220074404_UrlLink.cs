using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class UrlLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0fffbd72-8e33-49c3-a540-2d5d6469eea6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f659606-a41f-43e8-8241-d80429fbabf2");

            migrationBuilder.AddColumn<string>(
                name: "UrlMovie",
                table: "Movie",
                type: "VARCHAR(500)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Comment",
                type: "VARCHAR(400)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(150)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3eb9dd07-bb9e-4740-bfe3-f2f570055e57", null, "User", "USER" },
                    { "7f9b66fd-6cae-4570-9e60-12b2cec62064", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3eb9dd07-bb9e-4740-bfe3-f2f570055e57");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7f9b66fd-6cae-4570-9e60-12b2cec62064");

            migrationBuilder.DropColumn(
                name: "UrlMovie",
                table: "Movie");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Comment",
                type: "VARCHAR(150)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(400)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0fffbd72-8e33-49c3-a540-2d5d6469eea6", null, "Admin", "ADMIN" },
                    { "5f659606-a41f-43e8-8241-d80429fbabf2", null, "User", "USER" }
                });
        }
    }
}
