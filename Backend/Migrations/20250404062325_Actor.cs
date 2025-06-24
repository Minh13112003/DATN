using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class Actor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3f3a432e-16bb-4f03-9b83-f36f0bcf8201");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6d8b0fd9-e709-4fe9-87ab-088a7619fde9");

            migrationBuilder.AddColumn<bool>(
                name: "Block",
                table: "Movie",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVip",
                table: "Movie",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NameActor",
                table: "Movie",
                type: "VARCHAR(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Actor",
                columns: table => new
                {
                    IdActor = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    ActorName = table.Column<string>(type: "VARCHAR(350)", nullable: false),
                    Birthday = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actor", x => x.IdActor);
                });

            migrationBuilder.CreateTable(
                name: "SubActor",
                columns: table => new
                {
                    IdMovie = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    IdActor = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubActor", x => new { x.IdMovie, x.IdActor });
                    table.ForeignKey(
                        name: "FK_SubActor_Actor_IdActor",
                        column: x => x.IdActor,
                        principalTable: "Actor",
                        principalColumn: "IdActor",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubActor_Movie_IdMovie",
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
                    { "305456bd-48de-4010-9e85-a3da28ec18d4", null, "Admin", "ADMIN" },
                    { "7a02e8a0-f90d-4dd6-9e48-a1255d9e69e8", null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubActor_IdActor",
                table: "SubActor",
                column: "IdActor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubActor");

            migrationBuilder.DropTable(
                name: "Actor");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "305456bd-48de-4010-9e85-a3da28ec18d4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7a02e8a0-f90d-4dd6-9e48-a1255d9e69e8");

            migrationBuilder.DropColumn(
                name: "Block",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "IsVip",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "NameActor",
                table: "Movie");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3f3a432e-16bb-4f03-9b83-f36f0bcf8201", null, "Admin", "ADMIN" },
                    { "6d8b0fd9-e709-4fe9-87ab-088a7619fde9", null, "User", "USER" }
                });
        }
    }
}
