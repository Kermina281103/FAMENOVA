using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace famenova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OtpCodes_UserId_Purpose",
                table: "OtpCodes");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "OtpCodes");

            migrationBuilder.DropColumn(
                name: "ConsumedAtUtc",
                table: "OtpCodes");

            migrationBuilder.DropColumn(
                name: "Destination",
                table: "OtpCodes");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "OtpCodes");

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "OtpCodes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_OtpCodes_UserId",
                table: "OtpCodes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OtpCodes_UserId",
                table: "OtpCodes");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "OtpCodes");

            migrationBuilder.AddColumn<int>(
                name: "Channel",
                table: "OtpCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConsumedAtUtc",
                table: "OtpCodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "OtpCodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Purpose",
                table: "OtpCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OtpCodes_UserId_Purpose",
                table: "OtpCodes",
                columns: new[] { "UserId", "Purpose" });
        }
    }
}
