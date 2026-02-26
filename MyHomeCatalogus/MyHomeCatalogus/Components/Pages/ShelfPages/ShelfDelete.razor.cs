using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.ShelfPages
{
    public partial class ShelfDelete
    {
        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required IShelfService ShelfService { get; set; }

        [Inject]
        public required IToastService ToastService { get; set; }

        [Parameter]
        public int Id { get; set; }

        private string? _message;
        private bool _isProcessing;
        private Shelf? _shelf;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _shelf ??= await ShelfService.Get(Id);
            }
            catch (KeyNotFoundException kex)
            {
                Console.WriteLine(kex);
                _message = "Shelf not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching shelf: {ex.Message}";
            }
        }

        private async Task DeleteShelf()
        {
            if (_isProcessing)
            {
                return;
            }

            try
            {
                _isProcessing = true;
                _message = null;

                await ShelfService.Delete(Id);

                ToastService.ShowToast($"Shelf '{_shelf?.ToDisplayString()}' was successfully removed.", ToastLevel.Success);

                NavigationManager.NavigateTo(RouteConstants.ShelfBaseRoute);
            }
            catch (Exception ex)
            {
                _isProcessing = false;

                Console.WriteLine(ex);

                ToastService.ShowToast($"Error deleting shelf: {ex.Message}", ToastLevel.Error, true);
            }
        }

    }
}
