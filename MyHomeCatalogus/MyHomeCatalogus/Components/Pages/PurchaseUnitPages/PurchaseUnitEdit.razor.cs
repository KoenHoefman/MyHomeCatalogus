using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.PurchaseUnitPages
{
	public partial class PurchaseUnitEdit
	{
		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		[Inject]
		public required IPurchaseUnitService PurchaseUnitService { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[Parameter]
		public int Id { get; set; }

		[SupplyParameterFromForm]
		private PurchaseUnit? PurchaseUnit { get; set; }

		private string? _message;
		private bool _isProcessing;

		private EditContext EditContext { get; set; } = null!;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				PurchaseUnit ??= await PurchaseUnitService.Get(Id);

				EditContext = new EditContext(PurchaseUnit);
			}
			catch (KeyNotFoundException)
			{
				_message = "Purchase unit not found.";
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching data: {ex.Message}";
			}

		}

		private async Task UpdatePurchaseUnit()
		{
			if (_isProcessing)
			{
				return;
			}

			try
			{
				_isProcessing = true;
				_message = null;

				var updatedEntity = await PurchaseUnitService.Update(PurchaseUnit!);

				ToastService.ShowToast($"Purchase unit '{updatedEntity.Name}' was successfully updated.", ToastLevel.Success);

				NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.PurchaseUnitBaseRoute, updatedEntity.Id));
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

				ToastService.ShowToast($"Error updating purchase unit: {ex.Message}", ToastLevel.Error, true);
			}
		}

	}
}
