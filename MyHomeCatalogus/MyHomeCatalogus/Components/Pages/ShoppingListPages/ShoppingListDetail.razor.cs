using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using MyHomeCatalogus.Components.Forms;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

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
		private AddProductModal _addProductModal = null!;
		private EditItemModal _editItemModal = null!;
		private ConfirmationModal<ShoppingListItem> _deleteModal = null!;
		private QuickGrid<ShoppingListItem>? _grid;
		private bool _isLoading;


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
				_isLoading = true;
				_message = null;

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
			finally
			{
				_isLoading = false;
			}
		}

	}
}
