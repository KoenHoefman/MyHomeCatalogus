using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.StockUnitPages
{
    public partial class StockUnitDetail
    {
        [Inject]
        public required IStockUnitService StockUnitService { get; set; }

        [Parameter]
        public int Id { get; set; }

        private StockUnit? _stockUnit;
        private string? _message;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _stockUnit ??= await StockUnitService.Get(Id);
            }
            catch (KeyNotFoundException kex)
            {
                Console.WriteLine(kex);
                _message = "Stock unit not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data. {ex.Message}";
            }
        }
    }
}