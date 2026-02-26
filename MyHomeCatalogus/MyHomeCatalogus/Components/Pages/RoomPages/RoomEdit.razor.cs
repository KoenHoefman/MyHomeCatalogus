using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.RoomPages
{
    public partial class RoomEdit
    {
        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required IRoomService RoomService { get; set; }

        [Inject]
        public required IToastService ToastService { get; set; }

        [Parameter]
        public int Id { get; set; }

        [SupplyParameterFromForm]
        private Room? Room { get; set; }

        private string? _message = null;
        private bool _isProcessing;

        private EditContext EditContext { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Room ??= await RoomService.Get(Id);

                EditContext = new EditContext(Room);
            }
            catch (KeyNotFoundException kex)
            {
                _message = "Room not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data: {ex.Message}";
            }
        }

        private async Task UpdateRoom()
        {
            if (_isProcessing)
            {
                return;
            }

            try
            {
                _isProcessing = true;
                _message = null;

                var updatedEntity = await RoomService.Update(Room!);

                ToastService.ShowToast($"Room '{updatedEntity.Name}' was successfully updated.", ToastLevel.Success);

                NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.RoomBaseRoute, updatedEntity.Id));
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

                ToastService.ShowToast($"Error updating room: {ex.Message}", ToastLevel.Error, true);
            }
        }

    }
}
