using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeCatalogus.Migrations
{
    /// <inheritdoc />
    public partial class AuditStockItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "stockitem_audits",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    stockitem_id = table.Column<int>(type: "int", nullable: false),
                    audit_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    quantity_old = table.Column<int>(type: "int", nullable: false),
                    quantity_new = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stockitem_audits", x => x.id);
                    table.CheckConstraint("CK_stockitem_audits_quantity_new_zero_or_positive", "[quantity_new] >= 0");
                    table.CheckConstraint("CK_stockitem_audits_quantity_old_zero_or_positive", "[quantity_old] >= 0");
                    table.ForeignKey(
                        name: "FK_stockitem_audits_stockitems",
                        column: x => x.stockitem_id,
                        principalTable: "stockitems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_stockitem_audits_stockitem_id",
                table: "stockitem_audits",
                column: "stockitem_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stockitem_audits");
        }
    }
}
