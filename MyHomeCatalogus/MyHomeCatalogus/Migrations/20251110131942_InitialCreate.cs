using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeCatalogus.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "product_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "purchaseunits",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchaseunits", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "recipe_ingredient_measurements",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    abbreviation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_ingredient_measurements", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "recipes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stockunits",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stockunits", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stores",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stores", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "recipe_preparationsteps",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    recipe_id = table.Column<int>(type: "int", nullable: false),
                    step_nr = table.Column<int>(type: "int", nullable: false),
                    instructions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_preparationsteps", x => x.id);
                    table.CheckConstraint("CK_recipe_preparationsteps_step_nr_positive", "[step_nr] > 0");
                    table.ForeignKey(
                        name: "FK_recipe_preparationsteps_recipes",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "storageunits",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    room_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_storageunits", x => x.id);
                    table.ForeignKey(
                        name: "FK_storageunits_rooms",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    picture = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    picture_mime_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    barcode = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    barcode_mime_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    product_type_id = table.Column<int>(type: "int", nullable: false),
                    store_id = table.Column<int>(type: "int", nullable: false),
                    purchaseunit_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                    table.CheckConstraint("CK_products_barcode_mime_type_required", "(([barcode] IS NULL AND [barcode_mime_type] IS NULL) OR ([barcode] IS NOT NULL AND [barcode_mime_type] IS NOT NULL))");
                    table.CheckConstraint("CK_products_picture_mime_type_required", "(([picture] IS NULL AND [picture_mime_type] IS NULL) OR ([picture] IS NOT NULL AND [picture_mime_type] IS NOT NULL))");
                    table.ForeignKey(
                        name: "FK_product_types_products",
                        column: x => x.product_type_id,
                        principalTable: "product_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchaseunits_products",
                        column: x => x.purchaseunit_id,
                        principalTable: "purchaseunits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stores_products",
                        column: x => x.store_id,
                        principalTable: "stores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shoppinglists",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    store_id = table.Column<int>(type: "int", nullable: false),
                    date_created = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "CONVERT(date, GETDATE())"),
                    is_completed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shoppinglists", x => x.id);
                    table.ForeignKey(
                        name: "FK_stores_shoppinglists",
                        column: x => x.store_id,
                        principalTable: "stores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shelves",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    storageunit_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shelves", x => x.id);
                    table.ForeignKey(
                        name: "FK_shelves_storageunits",
                        column: x => x.storageunit_id,
                        principalTable: "storageunits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "product_thresholds",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    threshold = table.Column<int>(type: "int", nullable: false),
                    purchase_quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_thresholds", x => x.id);
                    table.CheckConstraint("CK_productthresholds_purchasequantity_positive", "[purchase_quantity] > 0");
                    table.CheckConstraint("CK_productthresholds_threshold_zero_or_positive", "[threshold] >= 0");
                    table.ForeignKey(
                        name: "FK_product_thresholds_products",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_ingredients",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    recipe_Preparationstep_Id = table.Column<int>(type: "int", nullable: false),
                    fooproduct_Id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    measurement_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_ingredients", x => x.id);
                    table.CheckConstraint("CK_recipe_ingredients_quantity_positive", "[quantity] > 0");
                    table.ForeignKey(
                        name: "FK_products_recipe_ingredients",
                        column: x => x.fooproduct_Id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_recipe_ingredient_measurements_recipe_ingredients",
                        column: x => x.measurement_id,
                        principalTable: "recipe_ingredient_measurements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_recipe_preparationsteps_recipe_ingredients",
                        column: x => x.recipe_Preparationstep_Id,
                        principalTable: "recipe_preparationsteps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shoppinglist_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    shoppinglist_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shoppinglist_items", x => x.id);
                    table.CheckConstraint("CK_shoppinglist_items_quantity_positive", "[quantity] > 0");
                    table.ForeignKey(
                        name: "FK_products_shoppinglist_items",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_shoppinglist_items_shoppinglists",
                        column: x => x.shoppinglist_id,
                        principalTable: "shoppinglists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stockitems",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    shelf_id = table.Column<int>(type: "int", nullable: false),
                    stockunit_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stockitems", x => x.id);
                    table.CheckConstraint("CK_stockitems_quantity_zero_or_positive", "[quantity] >= 0");
                    table.ForeignKey(
                        name: "FK_products_stockitems",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_shelves_stockitems",
                        column: x => x.shelf_id,
                        principalTable: "shelves",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stockunits_stockitems",
                        column: x => x.stockunit_id,
                        principalTable: "stockunits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_product_thresholds_product_id",
                table: "product_thresholds",
                column: "product_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_product_types_name",
                table: "product_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_producttypeid",
                table: "products",
                column: "product_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_purchaseunit_id",
                table: "products",
                column: "purchaseunit_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_store_id",
                table: "products",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "UQ_purchaseunits_name",
                table: "purchaseunits",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_recipe_ingredient_measurements_abbreviation",
                table: "recipe_ingredient_measurements",
                column: "abbreviation",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_recipe_ingredient_measurements_name",
                table: "recipe_ingredient_measurements",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recipe_ingredients_fooproduct_Id",
                table: "recipe_ingredients",
                column: "fooproduct_Id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_ingredients_measurement_id",
                table: "recipe_ingredients",
                column: "measurement_id");

            migrationBuilder.CreateIndex(
                name: "UQ_recipe_ingredients_recipe_preparationstep_id_product_id",
                table: "recipe_ingredients",
                columns: new[] { "recipe_Preparationstep_Id", "fooproduct_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_recipe_preparationsteps_recipe_id_step_nr",
                table: "recipe_preparationsteps",
                columns: new[] { "recipe_id", "step_nr" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_recipes_name",
                table: "recipes",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_rooms_name",
                table: "rooms",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_shelves_storageunit_id",
                table: "shelves",
                column: "storageunit_id");

            migrationBuilder.CreateIndex(
                name: "UQ_shelves_name_storageunit_id",
                table: "shelves",
                columns: new[] { "name", "storageunit_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_shoppinglist_items_product_id",
                table: "shoppinglist_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "UQ_shoppinglist_items_shoppinglist_id_product_id",
                table: "shoppinglist_items",
                columns: new[] { "shoppinglist_id", "product_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_shoppinglists_store_id_one_active_list",
                table: "shoppinglists",
                column: "store_id",
                unique: true,
                filter: "[is_completed] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_products_stockitems",
                table: "stockitems",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_stockitems_shelf_id",
                table: "stockitems",
                column: "shelf_id");

            migrationBuilder.CreateIndex(
                name: "IX_stockitems_stockunit_id",
                table: "stockitems",
                column: "stockunit_id");

            migrationBuilder.CreateIndex(
                name: "UQ_stockunits_name",
                table: "stockunits",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_storageunits_room_id",
                table: "storageunits",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "UQ_storageunits_name_room_id",
                table: "storageunits",
                columns: new[] { "name", "room_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_stores_name",
                table: "stores",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_thresholds");

            migrationBuilder.DropTable(
                name: "recipe_ingredients");

            migrationBuilder.DropTable(
                name: "shoppinglist_items");

            migrationBuilder.DropTable(
                name: "stockitems");

            migrationBuilder.DropTable(
                name: "recipe_ingredient_measurements");

            migrationBuilder.DropTable(
                name: "recipe_preparationsteps");

            migrationBuilder.DropTable(
                name: "shoppinglists");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "shelves");

            migrationBuilder.DropTable(
                name: "stockunits");

            migrationBuilder.DropTable(
                name: "recipes");

            migrationBuilder.DropTable(
                name: "product_types");

            migrationBuilder.DropTable(
                name: "purchaseunits");

            migrationBuilder.DropTable(
                name: "stores");

            migrationBuilder.DropTable(
                name: "storageunits");

            migrationBuilder.DropTable(
                name: "rooms");
        }
    }
}
