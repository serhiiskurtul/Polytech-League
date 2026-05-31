using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLastVisitAtToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastVisitAt",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastVisitAt",
                table: "users");
        }
    }
}
