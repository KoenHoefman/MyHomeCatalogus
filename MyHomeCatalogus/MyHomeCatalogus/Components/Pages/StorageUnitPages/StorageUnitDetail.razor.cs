using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.StorageUnitPages
{
	public partial class StorageUnitDetail
	{
		[Inject]
		public required IStorageUnitService StorageUnitService { get; set; }

		[Parameter]
		public int Id { get; set; }

		private StorageUnit? _storageUnit;
		private string? _message;

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
				_message = $"Error fetching data. {ex.Message}";
			}
		}
	}
}
