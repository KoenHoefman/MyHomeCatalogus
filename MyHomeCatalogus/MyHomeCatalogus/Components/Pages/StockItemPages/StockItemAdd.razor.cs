using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.StockItemPages
{
    public partial class StockItemAdd
    {
        [Inject]
        public required IStockItemService StockItemService { get; set; }

        [Inject]
        public required IProductService ProductService { get; set; }

        [Inject]
        public required IStockUnitService StockUnitService { get; set; }

        [Inject] 
        public required IShelfService ShelfService { get; set; }

        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required IToastService ToastService { get; set; }

        [SupplyParameterFromForm]
        private StockItem StockItem { get; set; } = new();

        private string? _message = null;
        private bool _isProcessing;

        private EditContext EditContext { get; set; } = null!;

        private IEnumerable<Product> _products = new List<Product>();
        private IEnumerable<StockUnit> _stockUnits = new List<StockUnit>();
        private IEnumerable<Shelf> _shelves = new List<Shelf>();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                EditContext = new EditContext(StockItem);

                var productsTask = ProductService.GetAll();
                var stockUnitsTask = StockUnitService.GetAll();
                var shelvesTask = ShelfService.GetAll();

                await Task.WhenAll(productsTask, stockUnitsTask, shelvesTask);

                _products = productsTask.Result;
                _stockUnits = stockUnitsTask.Result;
                _shelves = shelvesTask.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data: {ex.Message}";
            }
        }

        private async Task AddStockItem()
        {
            if (_isProcessing)
            {
                return;
            }

            try
            {
                _isProcessing = true;
                _message = null;

                var addedEntity = await StockItemService.Add(StockItem);

                ToastService.ShowToast($"Stock item was successfully added.", ToastLevel.Success);

                NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.StockItemBaseRoute, addedEntity.Id));
            }
            catch (UniqueConstraintException uex)
            {
                _isProcessing = false;

                EditContext.AddValidationErrors(uex);
            }
            catch (Exception ex)
            {
                _isProcessing = false;

                Console.WriteLine(ex);

                ToastService.ShowToast($"Error adding stock item: {ex.Message}", ToastLevel.Error, true);
            }
        }
    }

}

