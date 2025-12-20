using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Homy.Infurastructure.Migrations
{
    /// <inheritdoc />
    public partial class PerformanceOptimizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Properties",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Properties",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyTypes_IsDeleted",
                table: "PropertyTypes",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_IsDeleted",
                table: "Properties",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_IsDeleted",
                table: "Districts",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_IsDeleted",
                table: "Cities",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IsDeleted",
                table: "AspNetUsers",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PropertyTypes_IsDeleted",
                table: "PropertyTypes");

            migrationBuilder.DropIndex(
                name: "IX_Properties_IsDeleted",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Districts_IsDeleted",
                table: "Districts");

            migrationBuilder.DropIndex(
                name: "IX_Cities_IsDeleted",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IsDeleted",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,7)",
                oldPrecision: 10,
                oldScale: 7,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,7)",
                oldPrecision: 10,
                oldScale: 7,
                oldNullable: true);
        }
    }
}
