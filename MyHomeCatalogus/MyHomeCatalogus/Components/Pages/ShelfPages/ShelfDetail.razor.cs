using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.ShelfPages
{
	public partial class ShelfDetail
	{
		[Inject]
		public required IShelfService ShelfService { get; set; }

		[Parameter]
		public int Id { get; set; }

		private Shelf? _shelf;
		private string? _message;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				_shelf ??= await ShelfService.Get(Id);
			}
			catch (KeyNotFoundException kex)
			{
				Console.WriteLine(kex);
				_message = "Shelf not found.";
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching data. {ex.Message}";
			}
		}
	}
}
