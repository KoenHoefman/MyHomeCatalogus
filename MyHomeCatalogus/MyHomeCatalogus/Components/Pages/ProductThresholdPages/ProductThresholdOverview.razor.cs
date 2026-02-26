using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;

namespace MyHomeCatalogus.Components.Pages.ProductThresholdPages
{
    public partial class ProductThresholdOverview
    {
        [Inject]
        public required IProductThresholdService ProductThresholdService { get; set; }
    }
}
