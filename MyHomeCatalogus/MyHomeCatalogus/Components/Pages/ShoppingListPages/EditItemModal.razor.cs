using Microsoft.AspNetCore.Components;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Components.Pages.ShoppingListPages
{
    public partial class EditItemModal
    {

        [Inject]
        public required IShoppingListItemService ShoppingListItemService { get; set; }

        [Inject]
        public required IToastService ToastService { get; set; }

        [Parameter]
        public EventCallback OnSuccess { get; set; }

        private ShoppingListItem? _shoppingListItem;
        private bool _isVisible;
        private string? _errorMessage;


        public void OpenModal(ShoppingListItem shoppingListItem)
        {
            _shoppingListItem = shoppingListItem;

            _errorMessage = null;
            _isVisible = true;

            StateHasChanged();
        }

        public void CloseModal() => _isVisible = false;

        private async Task UpdateItem()
        {
            try
            {
                await ShoppingListItemService.Update(_shoppingListItem!);

                ToastService.ShowToast($"Item updated.", ToastLevel.Success);

                _isVisible = false;
                await OnSuccess.InvokeAsync();
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
                _errorMessage = $"Error updating item: {ex.Message}";
            }
        }
    }
}
