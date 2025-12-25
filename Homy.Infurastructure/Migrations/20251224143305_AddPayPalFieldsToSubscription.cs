using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Homy.Infurastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayPalFieldsToSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PayPalPayerId",
                table: "UserSubscriptions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayPalTransactionId",
                table: "UserSubscriptions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "UserSubscriptions",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayPalPayerId",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "PayPalTransactionId",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "UserSubscriptions");
        }
    }
}
