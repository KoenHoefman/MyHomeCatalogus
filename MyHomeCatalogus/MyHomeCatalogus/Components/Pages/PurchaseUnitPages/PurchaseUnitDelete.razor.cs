using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.PurchaseUnitPages
{
    public partial class PurchaseUnitDelete
    {
        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required IPurchaseUnitService PurchaseUnitService { get; set; }

        [Inject]
        public required IToastService ToastService { get; set; }

        [Parameter]
        public int Id { get; set; }

        private string? _message;
        private bool _isProcessing;
        private PurchaseUnit? _purchaseUnit;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _purchaseUnit ??= await PurchaseUnitService.Get(Id);
            }
            catch (KeyNotFoundException kex)
            {
                Console.WriteLine(kex);
                _message = "Purchase unit not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching purchase unit: {ex.Message}";
            }
        }

        private async Task DeletePurchaseUnit()
        {
            if (_isProcessing)
            {
                return;
            }

            try
            {
                _isProcessing = true;
                _message = null;

                await PurchaseUnitService.Delete(Id);

                ToastService.ShowToast($"Purchase unit '{_purchaseUnit?.Name}' was successfully removed.", ToastLevel.Success);

                NavigationManager.NavigateTo(RouteConstants.PurchaseUnitBaseRoute);
            }
            catch (Exception ex)
            {
                _isProcessing = false;

                Console.WriteLine(ex);

                ToastService.ShowToast($"Error deleting purchase unit: {ex.Message}", ToastLevel.Error, true);
            }
        }

    }
}
