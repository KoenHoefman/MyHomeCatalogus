using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.ProductThresholdPages
{
    public partial class ProductThresholdDetail
    {
        [Inject]
        public required IProductThresholdService ProductThresholdService { get; set; }

        [Parameter]
        public int Id { get; set; }

        private ProductThreshold? _productThreshold;
        private string? _message;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _productThreshold ??= await ProductThresholdService.Get(Id);
            }
            catch (KeyNotFoundException kex)
            {
                Console.WriteLine(kex);
                _message = "Product threshold not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data: {ex.Message}";
            }
        }
    }
}
