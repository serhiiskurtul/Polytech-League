using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADD_TIMING_COLUMNS_FIXED : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegistrationStart",
                table: "events",
                newName: "registration_start");

            migrationBuilder.RenameColumn(
                name: "RegistrationEnd",
                table: "events",
                newName: "registration_end");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "events",
                newName: "end_date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "registration_start",
                table: "events",
                newName: "RegistrationStart");

            migrationBuilder.RenameColumn(
                name: "registration_end",
                table: "events",
                newName: "RegistrationEnd");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "events",
                newName: "EndDate");
        }
    }
}
