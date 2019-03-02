using Microsoft.EntityFrameworkCore.Migrations;

namespace DatingApp.API.Migrations
{
    public partial class MessagesHidden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "RecipientDeleted",
                "Messages");

            migrationBuilder.RenameColumn(
                "SenderDeleted",
                "Messages",
                "RecipientHidden");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "RecipientHidden",
                "Messages",
                "SenderDeleted");

            migrationBuilder.AddColumn<bool>(
                "RecipientDeleted",
                "Messages",
                nullable: false,
                defaultValue: false);
        }
    }
}