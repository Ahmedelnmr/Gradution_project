using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Homy.Infurastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserVerificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdCardBackUrl",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdCardFrontUrl",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelfieWithIdUrl",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationRejectReason",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdCardBackUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdCardFrontUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SelfieWithIdUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VerificationRejectReason",
                table: "AspNetUsers");
        }
    }
}
