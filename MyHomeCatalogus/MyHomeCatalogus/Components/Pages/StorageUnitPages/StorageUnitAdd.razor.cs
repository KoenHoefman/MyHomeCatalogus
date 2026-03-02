using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.StorageUnitPages
{
	public partial class StorageUnitAdd
	{
		[Inject]
		public required IRoomService RoomService { get; set; }

		[Inject]
		public required IStorageUnitService StorageUnitService { get; set; }

		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[SupplyParameterFromForm]
		private StorageUnit StorageUnit { get; set; } = new();

		private string? _message;
		private bool _isProcessing;

		private EditContext EditContext { get; set; } = null!;

		private IEnumerable<Room> Rooms { get; set; } = new List<Room>();

		protected override async Task OnInitializedAsync()
		{
			try
			{
				EditContext = new EditContext(StorageUnit);

				Rooms = await RoomService.GetAll();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching data: {ex.Message}";
			}
		}

		private async Task AddStorageUnit()
		{
			if (_isProcessing)
			{
				return;
			}

			try
			{
				_isProcessing = true;
				_message = null;

				var addedEntity = await StorageUnitService.Add(StorageUnit);

				ToastService.ShowToast($"Storage unit '{addedEntity.Name}' was successfully added.", ToastLevel.Success);

				NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.StorageUnitBaseRoute, addedEntity.Id));
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

				ToastService.ShowToast($"Error adding storage unit: {ex.Message}", ToastLevel.Error, true);
			}
		}
	}
}
