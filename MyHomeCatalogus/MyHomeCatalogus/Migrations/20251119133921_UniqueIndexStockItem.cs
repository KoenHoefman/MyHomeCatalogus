using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeCatalogus.Migrations
{
    /// <inheritdoc />
    public partial class UniqueIndexStockItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UQ_stockitems_productid_shelfid",
                table: "stockitems",
                columns: new[] { "product_id", "shelf_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_stockitems_productid_shelfid",
                table: "stockitems");
        }
    }
}
