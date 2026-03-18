using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeCatalogus.Migrations
{
    /// <inheritdoc />
    public partial class UniqueIndexProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UQ_products_name_store_id",
                table: "products",
                columns: new[] { "name", "store_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_products_name_store_id",
                table: "products");
        }
    }
}
