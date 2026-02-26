using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;

namespace MyHomeCatalogus.Components.Pages.StockUnitPages
{
    public partial class StockUnitOverview
    {
        [Inject] public required IStockUnitService StockUnitService { get; set; }

    }
}
