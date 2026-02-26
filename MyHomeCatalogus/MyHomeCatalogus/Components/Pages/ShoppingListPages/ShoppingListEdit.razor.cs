using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.QuickGrid;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Forms;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;
using System;

namespace MyHomeCatalogus.Components.Pages.ShoppingListPages
{
    public partial class ShoppingListEdit
    {
        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required IShoppingListService ShoppingListService { get; set; }

        [Inject]
        public required IStoreService StoreService { get; set; }

        [Inject]
        public required IShoppingListItemService ShoppingListItemService { get; set; }

        [Inject]
        public required IToastService ToastService { get; set; }

        [Parameter]
        public int Id { get; set; }

        [SupplyParameterFromForm]
        private ShoppingList? ShoppingList { get; set; }

        private string? _message = null;
        private bool _isProcessing;

        private bool IsStoreLocked => _items.Any();

        private EditContext EditContext { get; set; } = null!;

        private IEnumerable<Store> Stores { get; set; } = new List<Store>();

        private IQueryable<ShoppingListItem> _items = (new List<ShoppingListItem>()).AsQueryable();

        private AddProductModal _addProductModal = null!;
        private EditItemModal _editItemModal = null!;
        private ConfirmationModal<ShoppingListItem> _deleteModal = null!;
        private QuickGrid<ShoppingListItem>? _grid = null;

        protected override async Task OnInitializedAsync()
        {
            if (ShoppingList is null)
            {
                await LoadData();
            }

            try
            {
                Stores = await StoreService.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data: {ex.Message}";
            }
        }

        private async Task UpdateShoppingList()
        {
            if (_isProcessing)
            {
                return;
            }

            try
            {
                _isProcessing = true;
                _message = null;

                var updatedEntity = await ShoppingListService.Update(ShoppingList!);

                ToastService.ShowToast($"Shopping list was successfully updated.", ToastLevel.Success);

                NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.ShoppingListBaseRoute, updatedEntity.Id));
            }
            catch (UniqueConstraintException uex)
            {
                _isProcessing = false;

                EditContext.AddValidationErrors(uex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error updating shopping list: {ex.Message}";
            }
        }

        private async Task HandleDelete(ShoppingListItem item)
        {
            try
            {
                await ShoppingListItemService.Delete(item.Id);

                ToastService.ShowToast($"{item.Product?.Name ?? "Item"} removed.", ToastLevel.Success);

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
                ShoppingList = await ShoppingListService.Get(Id);

                _items = ShoppingList.ShoppingListItems.Any() ?
                    ShoppingList.ShoppingListItems.AsQueryable() : new List<ShoppingListItem>().AsQueryable();

                EditContext = new EditContext(ShoppingList!);
            }
            catch (KeyNotFoundException kex)
            {
                Console.WriteLine(kex);
                _message = "Shopping list not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data: {ex.Message}";
            }
        }

    }
}
