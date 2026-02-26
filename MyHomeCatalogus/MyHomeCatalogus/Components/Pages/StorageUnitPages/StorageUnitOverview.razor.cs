using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.StorageUnitPages
{
    public partial class StorageUnitOverview
    {
        [Inject]
        public required IStorageUnitService StorageUnitService { get; set; }


        [Inject]
        public required IRoomService RoomService { get; set; }

        private string? _message = null;


        private IEnumerable<Room> _rooms = new List<Room>();


        protected override async Task OnInitializedAsync()
        {
            try
            {
                _rooms = await RoomService.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data: {ex.Message}";
            }
        }

    }
}
