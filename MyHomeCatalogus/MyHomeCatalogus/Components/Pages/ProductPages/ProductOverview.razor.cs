using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.ProductPages
{
    public partial class ProductOverview
    {
        [Inject]
        public required IProductService ProductService { get; set; }

        [Inject]
        public required IStoreService StoreService { get; set; }

        [Inject]
        public required IProductTypeService ProductTypeService { get; set; }

        [Inject]
        public required IShoppingListItemService ShoppingListItemService { get; set; }

        [Inject] 
        public required IToastService ToastService { get; set; }

        private string? _message = null;
        private bool _showModalAddToList = false;
        private Product? _selectedProduct;
        private int _quantity = 1;

        private IEnumerable<ProductType> _productTypes = new List<ProductType>();
        private IEnumerable<Store> _stores = new List<Store>();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var productTypesTask = ProductTypeService.GetAll();
                var storesTask = StoreService.GetAll();

                await Task.WhenAll(productTypesTask, storesTask);

                _productTypes = productTypesTask.Result;
                _stores = storesTask.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _message = $"Error fetching data: {ex.Message}";
            }
        }

        private void OpenModalAddToList(Product product)
        {
            _errorMessage = null;
            _selectedProduct = product;
            _quantity = 1;
            _showModalAddToList = true;
        }

        private void CloseModalAddToList() => _showModalAddToList = false;
        private string? _errorMessage;

        private async Task AddToList()
        {
            if (_selectedProduct != null)
            {
                _errorMessage = null;

                try
                {
                    await ShoppingListItemService.AddProduct(_selectedProduct.Id, _quantity);

                    CloseModalAddToList();

                    ToastService.ShowToast($"{_selectedProduct.Name} added to list.", ToastLevel.Success);
                }
                catch (UniqueConstraintException uex)
                {
                    _errorMessage = uex.ValidationErrors.Any()
                        ? string.Join(", ", uex.ValidationErrors.Select(e => e.ErrorMessage))
                        : uex.Message;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    _errorMessage = $"Error adding product: {ex.Message}";
                }

            }
        }

    }
}
