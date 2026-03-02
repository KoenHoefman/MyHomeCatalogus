using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.RoomPages
{
	public partial class RoomDelete
	{
		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		[Inject]
		public required IRoomService RoomService { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[Parameter]
		public int Id { get; set; }

		private string? _message;
		private bool _isProcessing;
		private Room? _room;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				_room ??= await RoomService.Get(Id);
			}
			catch (KeyNotFoundException kex)
			{
				Console.WriteLine(kex);
				_message = "Room not found.";
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching room: {ex.Message}";
			}
		}

		private async Task DeleteRoom()
		{
			if (_isProcessing)
			{
				return;
			}

			try
			{
				_isProcessing = true;
				_message = null;

				await RoomService.Delete(Id);

				ToastService.ShowToast($"Room '{_room?.Name}' was successfully removed.", ToastLevel.Success);

				NavigationManager.NavigateTo(RouteConstants.RoomBaseRoute);
			}
			catch (Exception ex)
			{
				_isProcessing = false;

				Console.WriteLine(ex);

				ToastService.ShowToast($"Error deleting room: {ex.Message}", ToastLevel.Error, true);
			}
		}

	}
}
