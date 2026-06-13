using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Barcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "BarcodeImage",
                table: "Products",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "QrCodeImage",
                table: "Products",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarcodeImage",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "QrCodeImage",
                table: "Products");
        }
    }
}
