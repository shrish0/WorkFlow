using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkFlow.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequisitionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequisitionHeaders",
                columns: table => new
                {
                    RequisitionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitionHeaders", x => x.RequisitionId);
                    table.ForeignKey(
                        name: "FK_RequisitionHeaders_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequisitionHeaders_SubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategories",
                        principalColumn: "SubCategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequisitionApprovals",
                columns: table => new
                {
                    RequisitionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SentTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitionApprovals", x => x.RequisitionId);
                    table.ForeignKey(
                        name: "FK_RequisitionApprovals_RequisitionHeaders_RequisitionId",
                        column: x => x.RequisitionId,
                        principalTable: "RequisitionHeaders",
                        principalColumn: "RequisitionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequisitionBodies",
                columns: table => new
                {
                    RequisitionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hasAttachment = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitionBodies", x => x.RequisitionId);
                    table.ForeignKey(
                        name: "FK_RequisitionBodies_RequisitionHeaders_RequisitionId",
                        column: x => x.RequisitionId,
                        principalTable: "RequisitionHeaders",
                        principalColumn: "RequisitionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequisitionSupplements",
                columns: table => new
                {
                    RequisitionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FileLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitionSupplements", x => x.RequisitionId);
                    table.ForeignKey(
                        name: "FK_RequisitionSupplements_RequisitionHeaders_RequisitionId",
                        column: x => x.RequisitionId,
                        principalTable: "RequisitionHeaders",
                        principalColumn: "RequisitionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequisitionHeaders_CategoryId",
                table: "RequisitionHeaders",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitionHeaders_SubCategoryId",
                table: "RequisitionHeaders",
                column: "SubCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequisitionApprovals");

            migrationBuilder.DropTable(
                name: "RequisitionBodies");

            migrationBuilder.DropTable(
                name: "RequisitionSupplements");

            migrationBuilder.DropTable(
                name: "RequisitionHeaders");
        }
    }
}
