using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.StorageUnitPages
{
	public partial class StorageUnitEdit
	{
		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		[Inject]
		public required IStorageUnitService StorageUnitService { get; set; }

		[Inject]
		public required IRoomService RoomService { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[Parameter]
		public int Id { get; set; }

		[SupplyParameterFromForm]
		private StorageUnit? StorageUnit { get; set; }

		private string? _message = null;
		private bool _isProcessing;

		private EditContext EditContext { get; set; } = null!;

		private IEnumerable<Room> _rooms = new List<Room>();

		protected override async Task OnInitializedAsync()
		{
			try
			{
				StorageUnit ??= await StorageUnitService.Get(Id);

				EditContext = new EditContext(StorageUnit);

				_rooms = await RoomService.GetAll();
			}
			catch (KeyNotFoundException)
			{
				_message = "Storage unit not found.";
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching data: {ex.Message}";
			}
		}

		private async Task UpdateStorageUnit()
		{
			if (_isProcessing)
			{
				return;
			}

			try
			{
				_isProcessing = true;
				_message = null;

				var updatedEntity = await StorageUnitService.Update(StorageUnit!);

				ToastService.ShowToast($"Storage unit '{updatedEntity.ToDisplayString()}' was successfully updated.", ToastLevel.Success);

				NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.StorageUnitBaseRoute, updatedEntity.Id));
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

				ToastService.ShowToast($"Error updating storage unit: {ex.Message}", ToastLevel.Error, true);
			}
		}

	}
}
