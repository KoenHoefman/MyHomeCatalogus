using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.StockItemPages
{
    public partial class StockItemDetail
    {
        [Inject]
        public required IStockItemService StockItemService { get; set; }

        [Parameter]
        public int Id { get; set; }

        private StockItem? _stockItem;
        private string? _message;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _stockItem ??= await StockItemService.Get(Id);
            }
            catch (KeyNotFoundException kex)
            {
                Console.WriteLine(kex);
                _message = "Stock item not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data. {ex.Message}";
            }
        }
    }
}
