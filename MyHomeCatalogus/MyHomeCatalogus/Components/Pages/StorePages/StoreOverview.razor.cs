using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;

namespace MyHomeCatalogus.Components.Pages.StorePages
{
    public partial class StoreOverview
    {
        [Inject]
        public required IStoreService StoreService { get; set; }

    }
}
