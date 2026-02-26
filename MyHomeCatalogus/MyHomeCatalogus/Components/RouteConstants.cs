using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components
{
    public static class RouteConstants
    {

        public static string GetAddRoute(string baseRoute) => $"{baseRoute}{AddSuffix}";
        public static string GetDeleteRoute(string baseRoute, int id) => GetRouteWithIdParameter($"{baseRoute}{DeleteSuffix}", id);
        public static string GetDetailRoute(string baseRoute, int id) => GetRouteWithIdParameter($"{baseRoute}{DetailSuffix}", id);
        public static string GetEditRoute(string baseRoute, int id) => GetRouteWithIdParameter($"{baseRoute}{EditSuffix}", id);
        public static string GetRouteWithIdParameter(string route, int id) => route.Replace("{id:int}", id.ToString());

        public const string AddSuffix = "/add";
        public const string DeleteSuffix = "/{id:int}/delete";
        public const string DetailSuffix = "/{id:int}";
        public const string EditSuffix = "/{id:int}/edit";

        public const string Home = "/";
        public const string About = "/about";

        #region Store

        public const string StoreBaseRoute = "/stores";
        public const string StoreAddRoute = $"{StoreBaseRoute}{AddSuffix}";
        public const string StoreDeleteRoute = $"{StoreBaseRoute}{DeleteSuffix}";
        public const string StoreDetailRoute = $"{StoreBaseRoute}{DetailSuffix}";
        public const string StoreEditRoute = $"{StoreBaseRoute}{EditSuffix}";

        #endregion

        #region ProductType

        public const string ProductTypeBaseRoute = "/producttypes";
        public const string ProductTypeAddRoute = $"{ProductTypeBaseRoute}{AddSuffix}";
        public const string ProductTypeDeleteRoute = $"{ProductTypeBaseRoute}{DeleteSuffix}";
        public const string ProductTypeDetailRoute = $"{ProductTypeBaseRoute}{DetailSuffix}";
        public const string ProductTypeEditRoute = $"{ProductTypeBaseRoute}{EditSuffix}";

        #endregion

        #region PurchaseUnit

        public const string PurchaseUnitBaseRoute = "/purchaseunits";
        public const string PurchaseUnitAddRoute = $"{PurchaseUnitBaseRoute}{AddSuffix}";
        public const string PurchaseUnitDeleteRoute = $"{PurchaseUnitBaseRoute}{DeleteSuffix}";
        public const string PurchaseUnitDetailRoute = $"{PurchaseUnitBaseRoute}{DetailSuffix}";
        public const string PurchaseUnitEditRoute = $"{PurchaseUnitBaseRoute}{EditSuffix}";

        #endregion

        #region StockUnit

        public const string StockUnitBaseRoute = "/stockunits";
        public const string StockUnitAddRoute = $"{StockUnitBaseRoute}{AddSuffix}";
        public const string StockUnitDeleteRoute = $"{StockUnitBaseRoute}{DeleteSuffix}";
        public const string StockUnitDetailRoute = $"{StockUnitBaseRoute}{DetailSuffix}";
        public const string StockUnitEditRoute = $"{StockUnitBaseRoute}{EditSuffix}";

        #endregion

        #region Room

        public const string RoomBaseRoute = "/rooms";
        public const string RoomAddRoute = $"{RoomBaseRoute}{AddSuffix}";
        public const string RoomDeleteRoute = $"{RoomBaseRoute}{DeleteSuffix}";
        public const string RoomDetailRoute = $"{RoomBaseRoute}{DetailSuffix}";
        public const string RoomEditRoute = $"{RoomBaseRoute}{EditSuffix}";

        #endregion

        #region StorageUnit

        public const string StorageUnitBaseRoute = "/storageunits";
        public const string StorageUnitAddRoute = $"{StorageUnitBaseRoute}{AddSuffix}";
        public const string StorageUnitDeleteRoute = $"{StorageUnitBaseRoute}{DeleteSuffix}";
        public const string StorageUnitDetailRoute = $"{StorageUnitBaseRoute}{DetailSuffix}";
        public const string StorageUnitEditRoute = $"{StorageUnitBaseRoute}{EditSuffix}";

        #endregion

        #region Shelf

        public const string ShelfBaseRoute = "/shelves";
        public const string ShelfAddRoute = $"{ShelfBaseRoute}{AddSuffix}";
        public const string ShelfDeleteRoute = $"{ShelfBaseRoute}{DeleteSuffix}";
        public const string ShelfDetailRoute = $"{ShelfBaseRoute}{DetailSuffix}";
        public const string ShelfEditRoute = $"{ShelfBaseRoute}{EditSuffix}";

        #endregion

        #region Product

        public const string ProductBaseRoute = "/products";
        public const string ProductAddRoute = $"{ProductBaseRoute}{AddSuffix}";
        public const string ProductDeleteRoute = $"{ProductBaseRoute}{DeleteSuffix}";
        public const string ProductDetailRoute = $"{ProductBaseRoute}{DetailSuffix}";
        public const string ProductEditRoute = $"{ProductBaseRoute}{EditSuffix}";

        #endregion

        #region ProductThreshold

        public const string ProductThresholdBaseRoute = "/productthresholds";
        public const string ProductThresholdAddRoute = $"{ProductThresholdBaseRoute}{AddSuffix}";
        public const string ProductThresholdDeleteRoute = $"{ProductThresholdBaseRoute}{DeleteSuffix}";
        public const string ProductThresholdDetailRoute = $"{ProductThresholdBaseRoute}{DetailSuffix}";
        public const string ProductThresholdEditRoute = $"{ProductThresholdBaseRoute}{EditSuffix}";

        #endregion

        #region StockItem

        public const string StockItemBaseRoute = "/stockitems";
        public const string StockItemAddRoute = $"{StockItemBaseRoute}{AddSuffix}";
        public const string StockItemDeleteRoute = $"{StockItemBaseRoute}{DeleteSuffix}";
        public const string StockItemDetailRoute = $"{StockItemBaseRoute}{DetailSuffix}";
        public const string StockItemEditRoute = $"{StockItemBaseRoute}{EditSuffix}";

        #endregion

        #region ShoppingList

        public const string ShoppingListBaseRoute = "/shoppinglists";
        public const string ShoppingListAddRoute = $"{ShoppingListBaseRoute}{AddSuffix}";
        public const string ShoppingListDeleteRoute = $"{ShoppingListBaseRoute}{DeleteSuffix}";
        public const string ShoppingListDetailRoute = $"{ShoppingListBaseRoute}{DetailSuffix}";
        public const string ShoppingListEditRoute = $"{ShoppingListBaseRoute}{EditSuffix}";

        #endregion
    }
}
