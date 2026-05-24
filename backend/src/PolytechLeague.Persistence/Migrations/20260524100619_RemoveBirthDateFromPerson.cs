using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBirthDateFromPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "birthday",
                table: "persons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "birthday",
                table: "persons",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
