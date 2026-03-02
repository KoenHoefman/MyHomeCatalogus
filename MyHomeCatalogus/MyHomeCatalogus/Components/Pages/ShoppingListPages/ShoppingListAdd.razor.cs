using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.ShoppingListPages
{
	public partial class ShoppingListAdd
	{

		[Inject]
		public required IShoppingListService ShoppingListService { get; set; }

		[Inject]
		public required IStoreService StoreService { get; set; }

		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[SupplyParameterFromForm]
		private ShoppingList ShoppingList { get; set; } = new ShoppingList() { DateCreated = DateOnly.FromDateTime(DateTime.Today) };

		private string? _message;
		private bool _isProcessing;

		private IEnumerable<Store> _stores = new List<Store>();

		private EditContext EditContext { get; set; } = null!;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				EditContext = new EditContext(ShoppingList);

				_stores = await StoreService.GetAll();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching data: {ex.Message}";
			}
		}

		private async Task AddShoppingList()
		{
			if (_isProcessing)
			{
				return;
			}

			try
			{
				_isProcessing = true;
				_message = null;

				var addedEntity = await ShoppingListService.Add(ShoppingList);

				ToastService.ShowToast($"Shopping list was successfully added.", ToastLevel.Success);

				NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.ShoppingListBaseRoute, addedEntity.Id));
			}
			catch (UniqueConstraintException uex)
			{
				_isProcessing = false;

				EditContext.AddValidationErrors(uex);
			}
			catch (Exception ex)
			{
				_isProcessing = false;

				Console.WriteLine(ex);

				ToastService.ShowToast($"Error adding shopping list: {ex.Message}", ToastLevel.Error, true);
			}
		}

	}
}
