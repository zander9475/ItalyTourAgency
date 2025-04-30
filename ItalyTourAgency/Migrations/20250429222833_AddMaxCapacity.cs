using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItalyTourAgency.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxCapacity",
                table: "Tour_Instance",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxCapacity",
                table: "Tour_Instance");
        }
    }
}
