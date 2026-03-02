using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.ProductTypePages
{
	public partial class ProductTypeDelete
	{
		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		[Inject]
		public required IProductTypeService ProductTypeService { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[Parameter]
		public int Id { get; set; }

		private string? _message;
		private bool _isProcessing;
		private ProductType? _productType;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				_productType ??= await ProductTypeService.Get(Id);
			}
			catch (KeyNotFoundException kex)
			{
				Console.WriteLine(kex);
				_message = "Product type not found.";
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching product type: {ex.Message}";
			}
		}

		private async Task DeleteProductType()
		{
			if (_isProcessing)
			{
				return;
			}

			try
			{
				_isProcessing = true;
				_message = null;

				await ProductTypeService.Delete(Id);

				ToastService.ShowToast($"Product type '{_productType?.Name}' was successfully removed.", ToastLevel.Success);

				NavigationManager.NavigateTo(RouteConstants.ProductTypeBaseRoute);
			}
			catch (Exception ex)
			{
				_isProcessing = false;

				Console.WriteLine(ex);

				ToastService.ShowToast($"Error deleting product type: {ex.Message}", ToastLevel.Error, true);
			}
		}

	}
}
