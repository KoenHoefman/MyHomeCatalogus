using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.RoomPages
{
	public partial class RoomDetail
	{
		[Inject]
		public required IRoomService RoomService { get; set; }

		[Parameter]
		public int Id { get; set; }

		private Room? _room;
		private string? _message;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				_room ??= await RoomService.Get(Id);
			}
			catch (KeyNotFoundException kex)
			{
				Console.WriteLine(kex);
				_message = "Room not found.";
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching data. {ex.Message}";
			}
		}
	}
}
