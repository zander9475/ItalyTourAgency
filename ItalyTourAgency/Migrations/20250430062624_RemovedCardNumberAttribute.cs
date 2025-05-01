using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItalyTourAgency.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCardNumberAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "card_number",
                table: "Booking");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "card_number",
                table: "Booking",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);
        }
    }
}
