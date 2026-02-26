using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.ProductTypePages
{
    public partial class ProductTypeOverview
    {
        [Inject]
        public required IProductTypeService ProductTypeService { get; set; }

    }
}
