using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;

namespace MyHomeCatalogus.Components.Pages.ProductTypePages
{
	public partial class ProductTypeOverview
	{
		[Inject]
		public required IProductTypeService ProductTypeService { get; set; }

	}
}
