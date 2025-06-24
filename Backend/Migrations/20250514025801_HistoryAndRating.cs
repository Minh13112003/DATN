using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class HistoryAndRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_Movie_IdMovie",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "Nation",
                table: "Actor");

            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    IdMovie = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => new { x.IdMovie, x.UserName });
                    table.ForeignKey(
                        name: "FK_History_Movie_IdMovie",
                        column: x => x.IdMovie,
                        principalTable: "Movie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieRating",
                columns: table => new
                {
                    IdMovie = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    RatePoint = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieRating", x => new { x.IdMovie, x.UserName });
                    table.ForeignKey(
                        name: "FK_MovieRating_Movie_IdMovie",
                        column: x => x.IdMovie,
                        principalTable: "Movie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Movie_IdMovie",
                table: "Report",
                column: "IdMovie",
                principalTable: "Movie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_Movie_IdMovie",
                table: "Report");

            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropTable(
                name: "MovieRating");

            migrationBuilder.AddColumn<string>(
                name: "Nation",
                table: "Actor",
                type: "VARCHAR(350)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Movie_IdMovie",
                table: "Report",
                column: "IdMovie",
                principalTable: "Movie",
                principalColumn: "Id");
        }
    }
}
