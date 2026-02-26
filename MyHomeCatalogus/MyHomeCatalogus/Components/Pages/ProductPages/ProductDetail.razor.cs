using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.ProductPages
{
    public partial class ProductDetail
    {
        [Inject]
        public required IProductService ProductService { get; set; }

        [Parameter]
        public int Id { get; set; }

        private Product? _product;
        private string? _message;

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
                _message = $"Error fetching product. {ex.Message}";
            }
        }
    }
}
