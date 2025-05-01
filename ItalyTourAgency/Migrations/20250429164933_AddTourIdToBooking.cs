using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItalyTourAgency.Migrations
{
    public partial class AddTourIdToBooking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "tour_id",
                table: "Booking",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Set existing records' TourId based on TourInstance
            migrationBuilder.Sql(@"
                UPDATE Booking
                SET tour_id = ti.tour_id
                FROM Booking b
                JOIN Tour_Instance ti ON b.tour_instance_id = ti.ID
            ");

            // Add foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Tour_TourId",
                table: "Booking",
                column: "tour_id",
                principalTable: "Tour",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // First drop the foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Tour_TourId",
                table: "Booking");

            // Then remove the column
            migrationBuilder.DropColumn(
                name: "tour_id",
                table: "Booking");

            // Restore original primary keys if needed
            // (Only include this if you actually modified PKs in Up)
            // migrationBuilder.AddPrimaryKey(...)
        }
    }
}