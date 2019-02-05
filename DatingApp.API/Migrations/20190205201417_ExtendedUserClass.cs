using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DatingApp.API.Migrations
{
    public partial class ExtendedUserClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "City",
                "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "Country",
                "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                "Created",
                "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                "DateOfBirth",
                "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                "Gender",
                "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "Interests",
                "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "Introduction",
                "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "KnownAs",
                "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                "LastActive",
                "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                "LookingFor",
                "Users",
                nullable: true);

            migrationBuilder.CreateTable(
                "Photos",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Url = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    IsMain = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        "FK_Photos_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_Photos_UserId",
                "Photos",
                "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Photos");

            migrationBuilder.DropColumn(
                "City",
                "Users");

            migrationBuilder.DropColumn(
                "Country",
                "Users");

            migrationBuilder.DropColumn(
                "Created",
                "Users");

            migrationBuilder.DropColumn(
                "DateOfBirth",
                "Users");

            migrationBuilder.DropColumn(
                "Gender",
                "Users");

            migrationBuilder.DropColumn(
                "Interests",
                "Users");

            migrationBuilder.DropColumn(
                "Introduction",
                "Users");

            migrationBuilder.DropColumn(
                "KnownAs",
                "Users");

            migrationBuilder.DropColumn(
                "LastActive",
                "Users");

            migrationBuilder.DropColumn(
                "LookingFor",
                "Users");
        }
    }
}