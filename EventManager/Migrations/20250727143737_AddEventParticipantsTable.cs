using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManager.Migrations
{
    /// <inheritdoc />
    public partial class AddEventParticipantsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "EventParticipants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "EventParticipants");
        }
    }
}
