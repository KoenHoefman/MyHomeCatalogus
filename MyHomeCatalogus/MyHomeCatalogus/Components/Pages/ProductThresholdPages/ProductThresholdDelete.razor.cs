using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.ProductThresholdPages
{
	public partial class ProductThresholdDelete
	{
		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		[Inject]
		public required IProductThresholdService ProductThresholdService { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[Parameter]
		public int Id { get; set; }

		private string? _message;
		private bool _isProcessing;
		private ProductThreshold? _productThreshold;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				_productThreshold ??= await ProductThresholdService.Get(Id);
			}
			catch (KeyNotFoundException kex)
			{
				Console.WriteLine(kex);
				_message = "Product treshold not found.";
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching product threshold: {ex.Message}";
			}
		}

		private async Task DeleteProductThreshold()
		{
			if (_isProcessing)
			{
				return;
			}

			try
			{
				_isProcessing = true;
				_message = null;

				await ProductThresholdService.Delete(Id);

				ToastService.ShowToast("Product threshold was successfully removed.", ToastLevel.Success);

				NavigationManager.NavigateTo(RouteConstants.ProductThresholdBaseRoute);
			}
			catch (Exception ex)
			{
				_isProcessing = false;

				Console.WriteLine(ex);

				ToastService.ShowToast($"Error deleting product threshold: {ex.Message}", ToastLevel.Error, true);
			}
		}

	}
}
