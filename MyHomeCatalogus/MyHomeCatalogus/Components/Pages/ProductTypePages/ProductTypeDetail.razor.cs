using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.ProductTypePages
{
	public partial class ProductTypeDetail
	{
		[Inject]
		public required IProductTypeService ProductTypeService { get; set; }

		[Parameter]
		public int Id { get; set; }

		private ProductType? _productType;
		private string? _message;

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
				_message = $"Error fetching data {ex.Message}";
			}
		}
	}
}