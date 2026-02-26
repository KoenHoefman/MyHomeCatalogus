using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.StorePages
{
    public partial class StoreAdd
    {
        [Inject]
        public required IStoreService StoreService { get; set; }

        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required IToastService ToastService { get; set; }

        [SupplyParameterFromForm]
        private Store Store { get; set; } = new();

        private string? _message = null;
        private bool _isProcessing;

        private EditContext EditContext { get; set; } = null!;

        protected override Task OnInitializedAsync()
        {
            EditContext = new EditContext(Store);

            return Task.CompletedTask;
        }

        private async Task AddStore()
        {
            if (_isProcessing)
            {
                return;
            }

            try
            {
                _isProcessing = true;
                _message = null;

                var addedEntity = await StoreService.Add(Store);

                ToastService.ShowToast($"Store '{addedEntity.Name}' was successfully added.", ToastLevel.Success);

                NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.StoreBaseRoute, addedEntity.Id));
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

                ToastService.ShowToast($"Error adding store: {ex.Message}", ToastLevel.Error, true);
            }
        }
    }
}
