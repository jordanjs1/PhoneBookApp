using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Contact.Api.Infrastructure.Migrations
{
    public partial class AddForeignKeyToContactInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ContactInformation_ContactId",
                table: "ContactInformation",
                column: "ContactId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactInformation_Contact_ContactId",
                table: "ContactInformation",
                column: "ContactId",
                principalTable: "Contact",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactInformation_Contact_ContactId",
                table: "ContactInformation");

            migrationBuilder.DropIndex(
                name: "IX_ContactInformation_ContactId",
                table: "ContactInformation");
        }
    }
}
