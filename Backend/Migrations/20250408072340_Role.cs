using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class Role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "305456bd-48de-4010-9e85-a3da28ec18d4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7a02e8a0-f90d-4dd6-9e48-a1255d9e69e8");

            migrationBuilder.RenameColumn(
                name: "NameActor",
                table: "Movie",
                newName: "NameDirector");

            migrationBuilder.AlterColumn<string>(
                name: "Birthday",
                table: "Actor",
                type: "VARCHAR(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)");

            migrationBuilder.AddColumn<string>(
                name: "SlugActorName",
                table: "Actor",
                type: "VARCHAR(350)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlImage",
                table: "Actor",
                type: "VARCHAR(200)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5ed666c5-637d-41f8-a7fe-e65130e8c622", null, "User", "USER" },
                    { "fd94bd31-e933-4f55-a9c4-885d178e6de3", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ed666c5-637d-41f8-a7fe-e65130e8c622");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fd94bd31-e933-4f55-a9c4-885d178e6de3");

            migrationBuilder.DropColumn(
                name: "SlugActorName",
                table: "Actor");

            migrationBuilder.DropColumn(
                name: "UrlImage",
                table: "Actor");

            migrationBuilder.RenameColumn(
                name: "NameDirector",
                table: "Movie",
                newName: "NameActor");

            migrationBuilder.AlterColumn<string>(
                name: "Birthday",
                table: "Actor",
                type: "VARCHAR(50)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "305456bd-48de-4010-9e85-a3da28ec18d4", null, "Admin", "ADMIN" },
                    { "7a02e8a0-f90d-4dd6-9e48-a1255d9e69e8", null, "User", "USER" }
                });
        }
    }
}
