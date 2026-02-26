using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.ShelfPages
{
    public partial class ShelfAdd
    {
        [Inject]
        public required IShelfService ShelfService { get; set; }

        [Inject]
        public required IStorageUnitService StorageUnitService { get; set; }

        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required IToastService ToastService { get; set; }

        [SupplyParameterFromForm]
        private Shelf Shelf { get; set; } = new();

        private string? _message = null;
        private bool _isProcessing;

        private EditContext EditContext { get; set; } = null!;

        private IEnumerable<StorageUnit> StorageUnits { get; set; } = new List<StorageUnit>();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                EditContext = new EditContext(Shelf);

                StorageUnits = await StorageUnitService.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data: {ex.Message}";
            }
        }

        private async Task AddShelf()
        {
            if (_isProcessing)
            {
                return;
            }

            try
            {
                _isProcessing = true;
                _message = null;

                var addedEntity = await ShelfService.Add(Shelf);

                ToastService.ShowToast($"Shelf '{addedEntity.Name}' was successfully added.", ToastLevel.Success);

                NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.ShelfBaseRoute, addedEntity.Id));
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

                ToastService.ShowToast($"Error adding shelf: {ex.Message}", ToastLevel.Error, true);
            }
        }
    }
}
