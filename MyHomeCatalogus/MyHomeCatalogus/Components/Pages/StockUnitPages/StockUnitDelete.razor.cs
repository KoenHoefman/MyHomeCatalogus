using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.StockUnitPages
{
    public partial class StockUnitDelete
    {
        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required IStockUnitService StockUnitService { get; set; }

        [Inject]
        public required IToastService ToastService { get; set; }

        [Parameter]
        public int Id { get; set; }

        private string? _message;
        private bool _isProcessing;
        private StockUnit? _stockUnit;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _stockUnit ??= await StockUnitService.Get(Id);
            }
            catch (KeyNotFoundException kex)
            {
                Console.WriteLine(kex);
                _message = "Stock unit not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching stock unit: {ex.Message}";
            }
        }

        private async Task DeleteStockUnit()
        {
            if (_isProcessing)
            {
                return;
            }

            try
            {
                _isProcessing = true;
                _message = null;

                await StockUnitService.Delete(Id);

                ToastService.ShowToast($"Stock unit '{_stockUnit?.Name}' was successfully removed.", ToastLevel.Success);

                NavigationManager.NavigateTo(RouteConstants.StockUnitBaseRoute);
            }
            catch (Exception ex)
            {
                _isProcessing = false;

                Console.WriteLine(ex);

                ToastService.ShowToast($"Error deleting stock unit: {ex.Message}", ToastLevel.Error, true);
            }
        }

    }
}
