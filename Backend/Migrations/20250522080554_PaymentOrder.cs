using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnTotNghiep.Migrations
{
    /// <inheritdoc />
    public partial class PaymentOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentOrder",
                columns: table => new
                {
                    IdPaymentOrder = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    UserName = table.Column<string>(type: "VARCHAR(150)", nullable: false),
                    OrderCode = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    TransactionId = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    Item = table.Column<string>(type: "VARCHAR(200)", nullable: false),
                    Status = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentOrder", x => x.IdPaymentOrder);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentOrder");
        }
    }
}
