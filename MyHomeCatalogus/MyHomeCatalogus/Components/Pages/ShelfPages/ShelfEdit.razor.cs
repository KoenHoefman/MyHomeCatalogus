using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.ShelfPages
{
	public partial class ShelfEdit
	{
		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		[Inject]
		public required IShelfService ShelfService { get; set; }

		[Inject]
		public required IStorageUnitService StorageUnitService { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[Parameter]
		public int Id { get; set; }

		[SupplyParameterFromForm]
		private Shelf? Shelf { get; set; }

		private string? _message;
		private bool _isProcessing;

		private EditContext EditContext { get; set; } = null!;

		private IEnumerable<StorageUnit> StorageUnits { get; set; } = new List<StorageUnit>();

		protected override async Task OnInitializedAsync()
		{
			try
			{
				Shelf ??= await ShelfService.Get(Id);

				EditContext = new EditContext(Shelf);

				StorageUnits = await StorageUnitService.GetAll();
			}
			catch (KeyNotFoundException)
			{
				_message = "Shelf not found.";
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching data: {ex.Message}";
			}
		}

		private async Task UpdateShelf()
		{
			if (_isProcessing)
			{
				return;
			}

			try
			{
				_isProcessing = true;
				_message = null;

				var updatedEntity = await ShelfService.Update(Shelf!);

				ToastService.ShowToast($"Shelf '{updatedEntity.ToDisplayString()}' was successfully updated.", ToastLevel.Success);

				NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.ShelfBaseRoute, updatedEntity.Id));
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

				ToastService.ShowToast($"Error updating shelf: {ex.Message}", ToastLevel.Error, true);
			}
		}

	}
}
