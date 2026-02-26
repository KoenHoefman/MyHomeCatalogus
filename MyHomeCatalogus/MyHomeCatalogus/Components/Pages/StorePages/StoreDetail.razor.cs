using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.StorePages
{
    public partial class StoreDetail
    {
        [Inject]
        public required IStoreService StoreService { get; set; }

        [Parameter]
        public int Id { get; set; }

        private Store? _store;
        private string? _message;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _store ??= await StoreService.Get(Id);
            }
            catch (KeyNotFoundException kex)
            {
                Console.WriteLine(kex);
                _message = "Store not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data.  {ex.Message}";
            }
        }
    }
}
