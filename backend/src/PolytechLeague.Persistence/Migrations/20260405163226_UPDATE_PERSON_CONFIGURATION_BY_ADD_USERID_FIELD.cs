using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UPDATE_PERSON_CONFIGURATION_BY_ADD_USERID_FIELD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "user_id",
                table: "persons",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_persons_user_id",
                table: "persons",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_persons_users_user_id",
                table: "persons",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_persons_users_user_id",
                table: "persons");

            migrationBuilder.DropIndex(
                name: "IX_persons_user_id",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "persons");
        }
    }
}
