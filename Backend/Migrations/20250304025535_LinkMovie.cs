using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class LinkMovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "UrlMovie",
                table: "Movie");

            migrationBuilder.CreateTable(
                name: "LinkMovie",
                columns: table => new
                {
                    IdLinkMovie = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    IdMovie = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    UrlMovie = table.Column<string>(type: "VARCHAR(400)", nullable: false),
                    Episode = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkMovie", x => x.IdLinkMovie);
                    table.ForeignKey(
                        name: "FK_LinkMovie_Movie_IdMovie",
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
                    { "4010c7d8-4cc4-43d2-8275-bac97484f762", null, "Admin", "ADMIN" },
                    { "924622f4-deb9-46c6-b9ea-ba6313e24da3", null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinkMovie_IdMovie",
                table: "LinkMovie",
                column: "IdMovie");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinkMovie");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4010c7d8-4cc4-43d2-8275-bac97484f762");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "924622f4-deb9-46c6-b9ea-ba6313e24da3");

            migrationBuilder.AddColumn<string>(
                name: "UrlMovie",
                table: "Movie",
                type: "VARCHAR(500)",
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
    }
}
