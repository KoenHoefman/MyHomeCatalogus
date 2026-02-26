using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;

namespace MyHomeCatalogus.Components.Pages.RoomPages
{
    public partial class RoomOverview
    {
        [Inject]
        public required IRoomService RoomService { get; set; }
    }
}
