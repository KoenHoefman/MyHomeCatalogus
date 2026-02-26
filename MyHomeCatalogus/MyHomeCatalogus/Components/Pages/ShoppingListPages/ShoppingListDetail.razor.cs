using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using MyHomeCatalogus.Components.Forms;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using System;

namespace MyHomeCatalogus.Components.Pages.ShoppingListPages
{
    public partial class ShoppingListDetail
    {
        [Inject]
        public required IShoppingListService ShoppingListService { get; set; }

        [Inject]
        public required IShoppingListItemService ShoppingListItemService { get; set; }

        [Inject]
        public required IToastService ToastService { get; set; }

        [Parameter]
        public int Id { get; set; }

        private ShoppingList? _shoppingList;
        private string? _message;

        private IQueryable<ShoppingListItem> _items = (new List<ShoppingListItem>()).AsQueryable();
        private AddProductModal _addProductModal = default;
        private EditItemModal _editItemModal = default;
        private ConfirmationModal<ShoppingListItem> _deleteModal = null!;
        private QuickGrid<ShoppingListItem>? _grid = null;

        protected override async Task OnInitializedAsync()
        {
            if (_shoppingList is null)
            {
                await LoadData();
            }
        }

        private async Task HandleDelete(ShoppingListItem item)
        {
            try
            {
                await ShoppingListItemService.Delete(item.Id);

                ToastService.ShowToast($"{item.Product?.Name} removed.", ToastLevel.Success);

                await RefreshData();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast($"Failed to delete item: {ex.Message}", ToastLevel.Error, true);
            }
        }


        private async Task RefreshData()
        {
            await LoadData();

            if (_grid is not null)
            {
                await _grid.RefreshDataAsync();
            }
        }


        private async Task LoadData()
        {
            try
            {
                _shoppingList = await ShoppingListService.Get(Id);

                _items = _shoppingList.ShoppingListItems.Any() ?
                    _shoppingList.ShoppingListItems.AsQueryable() : (new List<ShoppingListItem>()).AsQueryable();
            }
            catch (KeyNotFoundException kex)
            {
                Console.WriteLine(kex);
                _message = "Shopping list not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data.  {ex.Message}";
            }
        }

    }
}
