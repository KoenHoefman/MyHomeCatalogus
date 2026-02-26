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
    public partial class ProductTypeEdit
    {
        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required IProductTypeService ProductTypeService { get; set; }

        [Inject]
        public required IToastService ToastService { get; set; }

        [Parameter]
        public int Id { get; set; }

        [SupplyParameterFromForm]
        private ProductType? ProductType { get; set; }

        private string? _message = null;
        private bool _isProcessing;

        private EditContext EditContext { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ProductType ??= await ProductTypeService.Get(Id);

                EditContext = new EditContext(ProductType);
            }
            catch (KeyNotFoundException kex)
            {
                _message = "Product type not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data: {ex.Message}";
            }
        }

        private async Task UpdateProductType()
        {
            if (_isProcessing)
            {
                return;
            }

            try
            {
                _isProcessing = true;
                _message = null;

                var updatedEntity = await ProductTypeService.Update(ProductType!);

                ToastService.ShowToast($"Product type '{updatedEntity.Name}' was successfully updated.", ToastLevel.Success);

                NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.ProductTypeBaseRoute, updatedEntity.Id));
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

                ToastService.ShowToast($"Error updating product type: {ex.Message}", ToastLevel.Error, true);
            }
        }

    }
}
