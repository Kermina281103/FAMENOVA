using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace famenova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "PrescriptionOrder",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RejectReason",
                table: "PrescriptionOrder",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PrescriptionImageUrl",
                table: "PrescriptionOrder",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "PrescriptionOrder",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryAddress_street",
                table: "PrescriptionOrder",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryAddress_ZipCode",
                table: "PrescriptionOrder",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryAddress_City",
                table: "PrescriptionOrder",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<string>(
                name: "ClarificationImageUrl",
                table: "PrescriptionOrder",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClarificationRequest",
                table: "PrescriptionOrder",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClarificationRespondedAt",
                table: "PrescriptionOrder",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClarificationResponse",
                table: "PrescriptionOrder",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "PrescriptionOrder",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BatchNumber",
                table: "Batches",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "LogEntries",
                columns: table => new
                {
                    EventLogLevel = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionOrder_OrderId",
                table: "PrescriptionOrder",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionOrder_Status_CreatedOn",
                table: "PrescriptionOrder",
                columns: new[] { "Status", "CreatedOn" });

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionOrder_Order_OrderId",
                table: "PrescriptionOrder",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionOrder_Order_OrderId",
                table: "PrescriptionOrder");

            migrationBuilder.DropTable(
                name: "LogEntries");

            migrationBuilder.DropIndex(
                name: "IX_PrescriptionOrder_OrderId",
                table: "PrescriptionOrder");

            migrationBuilder.DropIndex(
                name: "IX_PrescriptionOrder_Status_CreatedOn",
                table: "PrescriptionOrder");

            migrationBuilder.DropColumn(
                name: "ClarificationImageUrl",
                table: "PrescriptionOrder");

            migrationBuilder.DropColumn(
                name: "ClarificationRequest",
                table: "PrescriptionOrder");

            migrationBuilder.DropColumn(
                name: "ClarificationRespondedAt",
                table: "PrescriptionOrder");

            migrationBuilder.DropColumn(
                name: "ClarificationResponse",
                table: "PrescriptionOrder");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "PrescriptionOrder");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "PrescriptionOrder",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "RejectReason",
                table: "PrescriptionOrder",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PrescriptionImageUrl",
                table: "PrescriptionOrder",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "PrescriptionOrder",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryAddress_street",
                table: "PrescriptionOrder",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryAddress_ZipCode",
                table: "PrescriptionOrder",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryAddress_City",
                table: "PrescriptionOrder",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BatchNumber",
                table: "Batches",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);
        }
    }
}
