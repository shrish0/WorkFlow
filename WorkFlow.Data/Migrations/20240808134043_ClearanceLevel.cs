using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkFlow.Data.Migrations
{
    /// <inheritdoc />
    public partial class ClearanceLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClearanceLevel",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Cl01");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClearanceLevel",
                table: "AspNetUsers");
        }
    }
}
