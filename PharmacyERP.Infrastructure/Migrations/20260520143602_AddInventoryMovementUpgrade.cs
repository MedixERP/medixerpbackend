using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryMovementUpgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovements_AspNetUsers_UserId",
                table: "InventoryMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovements_Products_ProductId",
                table: "InventoryMovements");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "InventoryMovements",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AddColumn<int>(
                name: "AfterQuantity",
                table: "InventoryMovements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BatchId",
                table: "InventoryMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BeforeQuantity",
                table: "InventoryMovements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReferenceId",
                table: "InventoryMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceType",
                table: "InventoryMovements",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_BatchId",
                table: "InventoryMovements",
                column: "BatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovements_AspNetUsers_UserId",
                table: "InventoryMovements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovements_ProductBatches_BatchId",
                table: "InventoryMovements",
                column: "BatchId",
                principalTable: "ProductBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovements_Products_ProductId",
                table: "InventoryMovements",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovements_AspNetUsers_UserId",
                table: "InventoryMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovements_ProductBatches_BatchId",
                table: "InventoryMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovements_Products_ProductId",
                table: "InventoryMovements");

            migrationBuilder.DropIndex(
                name: "IX_InventoryMovements_BatchId",
                table: "InventoryMovements");

            migrationBuilder.DropColumn(
                name: "AfterQuantity",
                table: "InventoryMovements");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "InventoryMovements");

            migrationBuilder.DropColumn(
                name: "BeforeQuantity",
                table: "InventoryMovements");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "InventoryMovements");

            migrationBuilder.DropColumn(
                name: "ReferenceType",
                table: "InventoryMovements");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "InventoryMovements",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovements_AspNetUsers_UserId",
                table: "InventoryMovements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovements_Products_ProductId",
                table: "InventoryMovements",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
