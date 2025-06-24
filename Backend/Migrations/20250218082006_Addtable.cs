using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class Addtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "055780d9-0293-4dda-b715-1f4da48b05d7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6ab5090d-0a1d-48eb-af93-78b671d43044");

            migrationBuilder.CreateTable(
                name: "Movie",
                columns: table => new
                {
                    Id = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Title = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(500)", nullable: false),
                    Nation = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    TypeMovie = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Status = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Catelogies = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    NumberOfMovie = table.Column<int>(type: "integer", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    Quality = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Language = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    View = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movie", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    IdComment = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    IdUserName = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    IdMovie = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Content = table.Column<string>(type: "VARCHAR(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.IdComment);
                    table.ForeignKey(
                        name: "FK_Comment_Movie_IdMovie",
                        column: x => x.IdMovie,
                        principalTable: "Movie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "735099ef-65b4-48e4-87c8-747f43ced3e2", null, "Admin", "ADMIN" },
                    { "cfe9fa0c-32a4-4d50-b62e-020c3dd424ce", null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_IdMovie",
                table: "Comment",
                column: "IdMovie");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Movie");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "735099ef-65b4-48e4-87c8-747f43ced3e2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cfe9fa0c-32a4-4d50-b62e-020c3dd424ce");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "055780d9-0293-4dda-b715-1f4da48b05d7", null, "Admin", "ADMIN" },
                    { "6ab5090d-0a1d-48eb-af93-78b671d43044", null, "User", "USER" }
                });
        }
    }
}
