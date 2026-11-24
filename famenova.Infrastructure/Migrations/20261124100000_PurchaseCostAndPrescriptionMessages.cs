using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace famenova.Infrastructure.Migrations
{
    /// <summary>
    /// D5: purchase cost (batch unit cost, product purchase cost, cost snapshot of each sale).
    /// Prescription conversation: PrescriptionMessages (existing clarification Q&amp;A copied in as history).
    /// </summary>
    public partial class PurchaseCostAndPrescriptionMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "Batches",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchaseCost",
                table: "Product",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CostAmount",
                table: "OrderItem",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "OrderItemBatches",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PrescriptionMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrescriptionOrderId = table.Column<int>(type: "int", nullable: false),
                    Sender = table.Column<int>(type: "int", nullable: false),
                    SenderUserId = table.Column<int>(type: "int", nullable: true),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionMessages_PrescriptionOrder_PrescriptionOrderId",
                        column: x => x.PrescriptionOrderId,
                        principalTable: "PrescriptionOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionMessages_PrescriptionOrderId_SentAtUtc",
                table: "PrescriptionMessages",
                columns: new[] { "PrescriptionOrderId", "SentAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionMessages_Sender_ReadAtUtc",
                table: "PrescriptionMessages",
                columns: new[] { "Sender", "ReadAtUtc" });

            // History: copy the existing clarification question / answer of each prescription into the thread.
            // Marked as read: they are old, they must not show up as new notifications.
            // Sender: 0 = Customer, 1 = Pharmacist.  Kind: 1 = ClarificationRequest, 2 = ClarificationReply.
            migrationBuilder.Sql(@"
INSERT INTO PrescriptionMessages (PrescriptionOrderId, Sender, SenderUserId, Kind, Body, AttachmentUrl, SentAtUtc, ReadAtUtc)
SELECT Id, 1, NULL, 1, ClarificationRequest, NULL, LastModifiedOn, LastModifiedOn
FROM PrescriptionOrder
WHERE ClarificationRequest IS NOT NULL;

INSERT INTO PrescriptionMessages (PrescriptionOrderId, Sender, SenderUserId, Kind, Body, AttachmentUrl, SentAtUtc, ReadAtUtc)
SELECT Id, 0, CustomerId, 2, ClarificationResponse, ClarificationImageUrl, ClarificationRespondedAt, ClarificationRespondedAt
FROM PrescriptionOrder
WHERE ClarificationRespondedAt IS NOT NULL
  AND (ClarificationResponse IS NOT NULL OR ClarificationImageUrl IS NOT NULL);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrescriptionMessages");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "PurchaseCost",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "CostAmount",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "OrderItemBatches");
        }
    }
}
