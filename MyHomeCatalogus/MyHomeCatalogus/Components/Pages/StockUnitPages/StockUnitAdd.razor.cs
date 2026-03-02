using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.StockUnitPages
{
	public partial class StockUnitAdd
	{
		[Inject]
		public required IStockUnitService StockUnitService { get; set; }

		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[SupplyParameterFromForm]
		private StockUnit StockUnit { get; set; } = new();

		private string? _message;
		private bool _isProcessing;

		private EditContext EditContext { get; set; } = null!;

		protected override Task OnInitializedAsync()
		{
			EditContext = new EditContext(StockUnit);
			return Task.CompletedTask;
		}

		private async Task AddStockUnit()
		{
			if (_isProcessing)
			{
				return;
			}

			try
			{
				_isProcessing = true;
				_message = null;

				var addedEntity = await StockUnitService.Add(StockUnit);

				ToastService.ShowToast($"Stock unit '{addedEntity.Name}' was successfully added.", ToastLevel.Success);

				NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.StockUnitBaseRoute, addedEntity.Id));
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

				ToastService.ShowToast($"Error adding stock unit: {ex.Message}", ToastLevel.Error, true);
			}
		}
	}
}
