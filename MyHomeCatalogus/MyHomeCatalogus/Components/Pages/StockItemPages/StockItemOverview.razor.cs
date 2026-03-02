using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;

namespace MyHomeCatalogus.Components.Pages.StockItemPages
{
	public partial class StockItemOverview
	{
		[Inject]
		public required IStockItemService StockItemService { get; set; }


	}
}
