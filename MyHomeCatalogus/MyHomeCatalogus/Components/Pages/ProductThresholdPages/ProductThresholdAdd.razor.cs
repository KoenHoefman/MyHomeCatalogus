using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Mono.TextTemplating;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.ProductThresholdPages
{
    public partial class ProductThresholdAdd
    {
        [Inject]
        public required IProductThresholdService ProductThresholdService { get; set; }

        [Inject]
        public required IProductService ProductService { get; set; }

        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required IToastService ToastService { get; set; }

        [SupplyParameterFromForm]
        private ProductThreshold ProductThreshold { get; set; } = new();

        private string? _message = null;
        private bool _isProcessing;

        private EditContext EditContext { get; set; } = null!;

        private IEnumerable<Product> Products { get; set; } = new List<Product>();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                EditContext = new EditContext(ProductThreshold);

                Products = await ProductService.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data: {ex.Message}";
            }
        }

        private async Task AddProductThreshold()
        {
            if (_isProcessing)
            {
                return;
            }

            try
            {
                _isProcessing = true;
                _message = null;

                var addedEntity = await ProductThresholdService.Add(ProductThreshold);

                ToastService.ShowToast($"Product threshold was successfully added.", ToastLevel.Success);

                NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.ProductThresholdBaseRoute, addedEntity.Id));
            }
            catch (UniqueConstraintException uex)
            {
                _isProcessing = false;

                EditContext.AddValidationErrors(uex);
            }
            catch (Exception ex)
            {
                _isProcessing = false;

                Console.WriteLine(ex);

                ToastService.ShowToast($"Error adding product threshold: {ex.Message}", ToastLevel.Error, true);
            }
        }
    }
}
