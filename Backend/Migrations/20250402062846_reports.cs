using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class reports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "498f5fc6-765f-449a-8da0-bd074471ef5d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84b656ad-4d6f-40ac-9b86-6764a98f670f");

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    IdReport = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    IdMovie = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    UserNameReporter = table.Column<string>(type: "VARCHAR(150)", nullable: false),
                    Content = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    UserNameAdminFix = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    Response = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    TimeReport = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    TimeResponse = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.IdReport);
                    table.ForeignKey(
                        name: "FK_Report_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Report_Movie_IdMovie",
                        column: x => x.IdMovie,
                        principalTable: "Movie",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "c5aa1141-642d-4455-83cd-c87dff42ae84", null, "User", "USER" },
                    { "fa0c4525-04c1-429a-9ad9-fdfb1c3a0d35", null, "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Report_IdMovie",
                table: "Report",
                column: "IdMovie");

            migrationBuilder.CreateIndex(
                name: "IX_Report_UserId",
                table: "Report",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c5aa1141-642d-4455-83cd-c87dff42ae84");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fa0c4525-04c1-429a-9ad9-fdfb1c3a0d35");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "498f5fc6-765f-449a-8da0-bd074471ef5d", null, "Admin", "ADMIN" },
                    { "84b656ad-4d6f-40ac-9b86-6764a98f670f", null, "User", "USER" }
                });
        }
    }
}
