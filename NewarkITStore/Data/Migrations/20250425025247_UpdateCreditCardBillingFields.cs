using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewarkITStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCreditCardBillingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BillingAddress",
                table: "CreditCards",
                newName: "BillingZip");

            migrationBuilder.AddColumn<string>(
                name: "BillingCity",
                table: "CreditCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BillingCountry",
                table: "CreditCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BillingState",
                table: "CreditCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingStreet",
                table: "CreditCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingCity",
                table: "CreditCards");

            migrationBuilder.DropColumn(
                name: "BillingCountry",
                table: "CreditCards");

            migrationBuilder.DropColumn(
                name: "BillingState",
                table: "CreditCards");

            migrationBuilder.DropColumn(
                name: "BillingStreet",
                table: "CreditCards");

            migrationBuilder.RenameColumn(
                name: "BillingZip",
                table: "CreditCards",
                newName: "BillingAddress");
        }
    }
}
