using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Contact.Api.Infrastructure.Migrations
{
    public partial class FixForeignKeyOnContactInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ContactInformation_ContactId",
                table: "ContactInformation");

            migrationBuilder.CreateIndex(
                name: "IX_ContactInformation_ContactId",
                table: "ContactInformation",
                column: "ContactId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ContactInformation_ContactId",
                table: "ContactInformation");

            migrationBuilder.CreateIndex(
                name: "IX_ContactInformation_ContactId",
                table: "ContactInformation",
                column: "ContactId",
                unique: true);
        }
    }
}
