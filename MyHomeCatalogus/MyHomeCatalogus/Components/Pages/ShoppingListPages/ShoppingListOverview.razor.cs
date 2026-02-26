using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Components.Forms;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.ShoppingListPages
{
    public partial class ShoppingListOverview
    {
        [Inject]
        public required IShoppingListService ShoppingListService { get; set; }

        [Inject]
        public required IStoreService StoreService { get; set; }

        private string? _message = null;
        private IEnumerable<Store> _stores = new List<Store>();

        private DataOverviewForm<ShoppingList> _overviewForm = null!; 
        private AddProductModal _addProductModal = default;


        protected override async Task OnInitializedAsync()
        {
            try
            {
                _stores = await StoreService.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data: {ex.Message}";
            }
        }


        private async Task RefreshData()
        {
            if (_overviewForm is not null)
            {
                await _overviewForm.RefreshAsync();
            }
        }
    }
}
