using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;

namespace MyHomeCatalogus.Components.Pages.PurchaseUnitPages
{
	public partial class PurchaseUnitOverview
	{
		[Inject] public required IPurchaseUnitService PurchaseUnitService { get; set; }

	}
}
