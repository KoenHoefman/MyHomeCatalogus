using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeCatalogus.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shelves_storageunits",
                table: "shelves");

            migrationBuilder.DropForeignKey(
                name: "FK_storageunits_rooms",
                table: "storageunits");

            migrationBuilder.AddForeignKey(
                name: "FK_shelves_storageunits",
                table: "shelves",
                column: "storageunit_id",
                principalTable: "storageunits",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_storageunits_rooms",
                table: "storageunits",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shelves_storageunits",
                table: "shelves");

            migrationBuilder.DropForeignKey(
                name: "FK_storageunits_rooms",
                table: "storageunits");

            migrationBuilder.AddForeignKey(
                name: "FK_shelves_storageunits",
                table: "shelves",
                column: "storageunit_id",
                principalTable: "storageunits",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_storageunits_rooms",
                table: "storageunits",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
