using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.ShelfPages
{
	public partial class ShelfOverview
	{
		[Inject]
		public required IShelfService ShelfService { get; set; }

		[Inject]
		public required IStorageUnitService StorageUnitService { get; set; }

		private string? _message;


		private IEnumerable<StorageUnit> _storageUnits = new List<StorageUnit>();


		protected override async Task OnInitializedAsync()
		{
			try
			{
				_storageUnits = await StorageUnitService.GetAll();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching data: {ex.Message}";
			}
		}

	}
}
