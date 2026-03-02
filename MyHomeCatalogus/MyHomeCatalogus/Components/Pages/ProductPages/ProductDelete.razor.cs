using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.ProductPages
{
	public partial class ProductDelete
	{
		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		[Inject]
		public required IProductService ProductService { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[Parameter]
		public int Id { get; set; }

		private string? _message;
		private bool _isProcessing;
		private Product? _product;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				_product ??= await ProductService.Get(Id);
			}
			catch (KeyNotFoundException kex)
			{
				Console.WriteLine(kex);
				_message = "Product not found.";
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching product: {ex.Message}";
			}
		}

		private async Task DeleteProduct()
		{
			if (_isProcessing)
			{
				return;
			}

			try
			{
				_isProcessing = true;
				_message = null;

				await ProductService.Delete(Id);

				ToastService.ShowToast($"Product '{_product?.Name}' was successfully removed.", ToastLevel.Success);

				NavigationManager.NavigateTo(RouteConstants.ProductBaseRoute);
			}
			catch (Exception ex)
			{
				_isProcessing = false;

				Console.WriteLine(ex);

				ToastService.ShowToast($"Error deleting product: {ex.Message}", ToastLevel.Error, true);
			}
		}

	}
}
