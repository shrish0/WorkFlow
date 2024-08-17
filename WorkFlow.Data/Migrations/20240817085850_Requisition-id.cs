using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkFlow.Data.Migrations
{
    /// <inheritdoc />
    public partial class Requisitionid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "hasAttachment",
                table: "RequisitionBodies",
                newName: "HasAttachment");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "RequisitionBodies",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "RequisitionBodies",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HasAttachment",
                table: "RequisitionBodies",
                newName: "hasAttachment");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "RequisitionBodies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "RequisitionBodies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
