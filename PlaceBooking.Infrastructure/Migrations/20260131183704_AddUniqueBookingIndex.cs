using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaceBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueBookingIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_SeatId",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SeatId_Date",
                table: "Bookings",
                columns: new[] { "SeatId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_SeatId_Date",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SeatId",
                table: "Bookings",
                column: "SeatId");
        }
    }
}
