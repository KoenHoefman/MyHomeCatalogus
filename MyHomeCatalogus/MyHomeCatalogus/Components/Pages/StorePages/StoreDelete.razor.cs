using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.StorePages
{
    public partial class StoreDelete
    {
        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required IStoreService StoreService { get; set; }

        [Inject]
        public required IToastService ToastService { get; set; }

        [Parameter]
        public int Id { get; set; }

        private string? _message;
        private bool _isProcessing;
        private Store? _store;


        protected override async Task OnInitializedAsync()
        {
            try
            {
                _store ??= await StoreService.Get(Id);
            }
            catch (KeyNotFoundException kex)
            {
                Console.WriteLine(kex);
                _message = "Store not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching store: {ex.Message}";
            }
        }

        private async Task DeleteStore()
        {
            if (_isProcessing)
            {
                return;
            }

            try
            {
                _isProcessing = true;
                _message = null;

                await StoreService.Delete(Id);

                ToastService.ShowToast($"Store '{_store?.Name}' was successfully removed.", ToastLevel.Success);

                NavigationManager.NavigateTo(RouteConstants.StoreBaseRoute);
            }
            catch (Exception ex)
            {
                _isProcessing = false;

                Console.WriteLine(ex);

                ToastService.ShowToast($"Error deleting store: {ex.Message}",ToastLevel.Error, true);
            }
        }

    }
}
