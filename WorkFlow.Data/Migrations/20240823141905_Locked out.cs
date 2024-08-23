using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkFlow.Data.Migrations
{
    /// <inheritdoc />
    public partial class Lockedout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "BlockedBy",
                table: "AspNetUsers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockedBy",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
