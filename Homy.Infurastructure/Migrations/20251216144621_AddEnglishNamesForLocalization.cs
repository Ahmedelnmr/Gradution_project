using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Homy.Infurastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEnglishNamesForLocalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "PropertyTypes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Districts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Cities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Amenities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "PropertyTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Amenities");
        }
    }
}
