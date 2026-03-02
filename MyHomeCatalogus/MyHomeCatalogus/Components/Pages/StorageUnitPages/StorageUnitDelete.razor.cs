using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.StorageUnitPages
{
	public partial class StorageUnitDelete
	{
		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		[Inject]
		public required IStorageUnitService StorageUnitService { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[Parameter]
		public int Id { get; set; }

		private string? _message;
		private bool _isProcessing;
		private StorageUnit? _storageUnit;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				_storageUnit ??= await StorageUnitService.Get(Id);
			}
			catch (KeyNotFoundException kex)
			{
				Console.WriteLine(kex);
				_message = "Storage unit not found.";
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching storage unit: {ex.Message}";
			}
		}

		private async Task DeleteStorageUnit()
		{
			if (_isProcessing)
			{
				return;
			}

			try
			{
				_isProcessing = true;
				_message = null;

				await StorageUnitService.Delete(Id);

				ToastService.ShowToast($"Storage unit '{_storageUnit?.ToDisplayString()}' was successfully removed.", ToastLevel.Success);

				NavigationManager.NavigateTo(RouteConstants.StorageUnitBaseRoute);
			}
			catch (Exception ex)
			{
				_isProcessing = false;

				Console.WriteLine(ex);

				ToastService.ShowToast($"Error deleting storage unit: {ex.Message}", ToastLevel.Error, true);
			}
		}

	}
}
