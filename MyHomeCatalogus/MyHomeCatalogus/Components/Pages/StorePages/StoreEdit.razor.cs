using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.StorePages
{
	public partial class StoreEdit
	{
		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		[Inject]
		public required IStoreService StoreService { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[Parameter]
		public int Id { get; set; }

		[SupplyParameterFromForm]
		private Store? Store { get; set; }

		private string? _message;
		private bool _isProcessing;

		private EditContext EditContext { get; set; } = null!;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				Store ??= await StoreService.Get(Id);

				EditContext = new EditContext(Store);
			}
			catch (KeyNotFoundException kex)
			{
				Console.WriteLine(kex);
				_message = "Store not found.";
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching data: {ex.Message}";
			}
		}

		private async Task UpdateStore()
		{
			if (_isProcessing)
			{
				return;
			}

			try
			{
				_isProcessing = true;
				_message = null;

				var updatedEntity = await StoreService.Update(Store!);

				ToastService.ShowToast($"Store '{updatedEntity.Name}' was successfully updated.", ToastLevel.Success);

				NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.StoreBaseRoute, updatedEntity.Id));
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

				ToastService.ShowToast($"Error updating store: {ex.Message}", ToastLevel.Error, true);
			}
		}

	}
}
