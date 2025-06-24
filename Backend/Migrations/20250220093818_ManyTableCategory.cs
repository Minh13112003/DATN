using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class ManyTableCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3eb9dd07-bb9e-4740-bfe3-f2f570055e57");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7f9b66fd-6cae-4570-9e60-12b2cec62064");

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    IdCategories = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    NameCategories = table.Column<string>(type: "VARCHAR(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.IdCategories);
                });

            migrationBuilder.CreateTable(
                name: "SubCategories",
                columns: table => new
                {
                    IdMovie = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    IdCategory = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategories", x => new { x.IdMovie, x.IdCategory });
                    table.ForeignKey(
                        name: "FK_SubCategories_Category_IdCategory",
                        column: x => x.IdCategory,
                        principalTable: "Category",
                        principalColumn: "IdCategories",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubCategories_Movie_IdMovie",
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
                    { "0f78deae-16de-4342-ba2d-2ab975ee664b", null, "Admin", "ADMIN" },
                    { "279e7f90-9bca-4882-9522-dfdaf5a9e5af", null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_IdCategory",
                table: "SubCategories",
                column: "IdCategory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubCategories");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0f78deae-16de-4342-ba2d-2ab975ee664b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "279e7f90-9bca-4882-9522-dfdaf5a9e5af");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3eb9dd07-bb9e-4740-bfe3-f2f570055e57", null, "User", "USER" },
                    { "7f9b66fd-6cae-4570-9e60-12b2cec62064", null, "Admin", "ADMIN" }
                });
        }
    }
}
