using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Mono.TextTemplating;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.PurchaseUnitPages
{
    public partial class PurchaseUnitAdd
    {
        [Inject]
        public required IPurchaseUnitService PurchaseUnitService { get; set; }

        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required IToastService ToastService { get; set; }

        [SupplyParameterFromForm]
        private PurchaseUnit PurchaseUnit { get; set; } = new();

        private string? _message = null;
        private bool _isProcessing;

        private EditContext EditContext { get; set; } = null!;
        
        protected override Task OnInitializedAsync()
        {
            EditContext = new EditContext(PurchaseUnit);
            return Task.CompletedTask;
        }

        private async Task AddPurchaseUnit()
        {
            if (_isProcessing)
            {
                return;
            }

            try
            {
                _isProcessing = true;
                _message = null;

                var addedEntity = await PurchaseUnitService.Add(PurchaseUnit);

                ToastService.ShowToast($"Purchase unit '{addedEntity.Name}' was successfully added.", ToastLevel.Success);

                NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.PurchaseUnitBaseRoute, addedEntity.Id));
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

                ToastService.ShowToast($"Error adding purchase unit: {ex.Message}", ToastLevel.Error, true);
            }
        }
    }
}
