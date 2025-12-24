using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNotes.Server.Migrations
{
    /// <inheritdoc />
    public partial class userRevert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGoogleUser",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGoogleUser",
                table: "Users");
        }
    }
}
