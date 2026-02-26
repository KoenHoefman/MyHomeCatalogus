using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.ProductTypePages
{
    public partial class ProductTypeAdd
    {

        [Inject]
        public required IProductTypeService ProductTypeService { get; set; }

        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required IToastService ToastService { get; set; }

        [SupplyParameterFromForm] 
        private ProductType ProductType { get; set; } = new();

        private EditContext EditContext { get; set; } = null!;

        private string? _message = null;
        private bool _isProcessing;

        protected override Task OnInitializedAsync()
        {
            EditContext = new EditContext(ProductType);
            return Task.CompletedTask;
        }

        private async Task AddProductType()
        {
            if (_isProcessing)
            {
                return;
            }

            try
            {
                _isProcessing = true;
                _message = null;

                var addedEntity = await ProductTypeService.Add(ProductType);

                ToastService.ShowToast($"Product type '{addedEntity.Name}' was successfully added.", ToastLevel.Success);

                NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.ProductTypeBaseRoute, addedEntity.Id));
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

                ToastService.ShowToast($"Error adding product type: {ex.Message}", ToastLevel.Error, true);
            }
        }
    }
}
