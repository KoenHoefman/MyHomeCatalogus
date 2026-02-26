using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.StockUnitPages
{
    public partial class StockUnitEdit
    {
        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required IStockUnitService StockUnitService { get; set; }

        [Inject]
        public required IToastService ToastService { get; set; }

        [Parameter]
        public int Id { get; set; }

        [SupplyParameterFromForm]
        private StockUnit? StockUnit { get; set; }

        private string? _message = null;
        private bool _isProcessing;

        private EditContext EditContext { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                StockUnit ??= await StockUnitService.Get(Id);

                EditContext = new EditContext(StockUnit);
            }
            catch (KeyNotFoundException kex)
            {
                _message = "Stock unit not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data: {ex.Message}";
            }
        }

        private async Task UpdateStockUnit()
        {
            if (_isProcessing)
            {
                return;
            }

            try
            {
                _isProcessing = true;
                _message = null;

                var updatedEntity = await StockUnitService.Update(StockUnit!);

                ToastService.ShowToast($"Stock unit '{updatedEntity.Name}' was successfully updated.", ToastLevel.Success);

                NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.StockUnitBaseRoute, updatedEntity.Id));
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

                ToastService.ShowToast($"Error updating stock unit: {ex.Message}", ToastLevel.Error, true);
            }
        }

    }
}
