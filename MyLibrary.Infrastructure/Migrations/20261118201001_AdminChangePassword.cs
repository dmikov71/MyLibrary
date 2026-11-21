using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLibrary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdminChangePassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPasswordResetRequired",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPasswordResetRequired",
                table: "AspNetUsers");
        }
    }
}
