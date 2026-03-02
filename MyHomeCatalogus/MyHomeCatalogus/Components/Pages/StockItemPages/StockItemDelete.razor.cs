using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.StockItemPages
{
	public partial class StockItemDelete
	{
		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		[Inject]
		public required IStockItemService StockItemService { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[Parameter]
		public int Id { get; set; }

		private string? _message;
		private bool _isProcessing;
		private StockItem? _stockItem;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				_stockItem ??= await StockItemService.Get(Id);
			}
			catch (KeyNotFoundException kex)
			{
				Console.WriteLine(kex);
				_message = "Stock item not found.";
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching stock item: {ex.Message}";
			}
		}

		private async Task DeleteStockItem()
		{
			if (_isProcessing)
			{
				return;
			}

			try
			{
				_isProcessing = true;
				_message = null;

				await StockItemService.Delete(Id);

				ToastService.ShowToast($"Stock item was successfully removed.", ToastLevel.Success);

				NavigationManager.NavigateTo(RouteConstants.StockItemBaseRoute);
			}
			catch (Exception ex)
			{
				_isProcessing = false;

				Console.WriteLine(ex);

				ToastService.ShowToast($"Error deleting stock item: {ex.Message}", ToastLevel.Error, true);
			}
		}

	}
}
