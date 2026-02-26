using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Pages.PurchaseUnitPages
{
    public partial class PurchaseUnitDetail
    {
        [Inject]
        public required IPurchaseUnitService PurchaseUnitService { get; set; }

        [Parameter]
        public int Id { get; set; }

        private PurchaseUnit? _purchaseUnit;
        private string? _message;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _purchaseUnit ??= await PurchaseUnitService.Get(Id);
            }
            catch (KeyNotFoundException kex)
            {
                Console.WriteLine(kex);
                _message = "Purchase unit not found.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data. {ex.Message}";
            }
        }
    }
}