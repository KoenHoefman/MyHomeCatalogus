using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.ShoppingListPages
{
	public partial class AddProductModal
	{
		[Inject]
		public required IProductService ProductService { get; set; }

		[Inject]
		public required IShoppingListItemService ShoppingListItemService { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[Parameter]
		public EventCallback OnSuccess { get; set; }

		private IEnumerable<Product>? _products = new List<Product>();

		private bool _isVisible;
		private int _quantity = 1;
		private string? _errorMessage;
		private int _selectedProductId;

		public async Task OpenModal(ShoppingList shoppingList)
		{
			_products = await ProductService.GetAll(p => p.StoreId == shoppingList.StoreId);

			_selectedProductId = 0;
			_quantity = 1;
			_errorMessage = null;
			_isVisible = true;

			StateHasChanged();
		}

		public void CloseModal() => _isVisible = false;

		private async Task AddProduct()
		{
			_errorMessage = null;

			try
			{
				await ShoppingListItemService.AddProduct(_selectedProductId, _quantity);

				ToastService.ShowToast($"Product added to list.", ToastLevel.Success);

				_isVisible = false;
				await OnSuccess.InvokeAsync();
			}
			catch (UniqueConstraintException uex)
			{
				_errorMessage = uex.ValidationErrors.Any()
					? string.Join(", ", uex.ValidationErrors.Select(e => e.ErrorMessage))
					: uex.Message;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_errorMessage = $"Error adding product: {ex.Message}";
			}
		}

	}
}
