using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.RoomPages
{
	public partial class RoomAdd
	{
		[Inject]
		public required IRoomService RoomService { get; set; }

		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[SupplyParameterFromForm]
		private Room Room { get; set; } = new();

		private string? _message;
		private bool _isProcessing;

		private EditContext EditContext { get; set; } = null!;

		protected override Task OnInitializedAsync()
		{
			EditContext = new EditContext(Room);
			return Task.CompletedTask;
		}

		private async Task AddRoom()
		{
			if (_isProcessing)
			{
				return;
			}

			try
			{
				_isProcessing = true;
				_message = null;

				var addedEntity = await RoomService.Add(Room);

				ToastService.ShowToast($"Room '{addedEntity.Name}' was successfully added.", ToastLevel.Success);

				NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.RoomBaseRoute, addedEntity.Id));
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

				ToastService.ShowToast($"Error adding room: {ex.Message}", ToastLevel.Error, true);
			}
		}
	}
}
