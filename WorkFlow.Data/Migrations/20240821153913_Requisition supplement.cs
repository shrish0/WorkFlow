using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkFlow.Data.Migrations
{
    /// <inheritdoc />
    public partial class Requisitionsupplement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileAddedBy",
                table: "RequisitionSupplements",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SentTo",
                table: "RequisitionApprovals",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileAddedBy",
                table: "RequisitionSupplements");

            migrationBuilder.AlterColumn<string>(
                name: "SentTo",
                table: "RequisitionApprovals",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
