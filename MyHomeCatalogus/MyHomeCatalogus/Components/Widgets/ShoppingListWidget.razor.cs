using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Widgets
{
    public partial class ShoppingListWidget
    {
        [Inject]
        public required IShoppingListService ShoppingListService { get; set; }

        public IEnumerable<ShoppingListWidgetData> Data { get; set; } = new List<ShoppingListWidgetData>();

        protected override async Task OnInitializedAsync()
        {
            Data = await ShoppingListService.GetItemsCountForActiveShoppingLists();
        }
    }
}
