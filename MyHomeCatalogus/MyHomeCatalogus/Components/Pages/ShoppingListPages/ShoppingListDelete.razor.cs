using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.ShoppingListPages
{
	public partial class ShoppingListDelete
	{

		[Inject]
		public required IShoppingListService ShoppingListService { get; set; }

		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[Parameter]
		public int Id { get; set; }

		private string? _message;
		private bool _isProcessing;
		private ShoppingList? _shoppingList;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				_shoppingList ??= await ShoppingListService.Get(Id);
			}
			catch (KeyNotFoundException kex)
			{
				Console.WriteLine(kex);
				_message = "Shopping list not found.";
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching shopping list: {ex.Message}";
			}
		}

		private async Task DeleteShoppingList()
		{
			if (_isProcessing)
			{
				return;
			}

			try
			{
				_isProcessing = true;
				_message = null;

				await ShoppingListService.Delete(Id);

				ToastService.ShowToast($"Shopping list was successfully removed.", ToastLevel.Success);

				NavigationManager.NavigateTo(RouteConstants.ShoppingListBaseRoute);
			}
			catch (Exception ex)
			{
				_isProcessing = false;

				Console.WriteLine(ex);

				ToastService.ShowToast($"Error deleting shopping list: {ex.Message}", ToastLevel.Error, true);
			}
		}

	}
}
