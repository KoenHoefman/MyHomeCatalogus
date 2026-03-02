using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Interfaces;

/// <summary>
/// Defines specialized operations for managing <see cref="ShoppingListItem"/> entities within the catalog.
/// </summary>
public interface IShoppingListItemService : IDataService<ShoppingListItem>
{
	/// <summary>
	/// Retrieves all items associated with a specific shopping list.
	/// </summary>
	/// <param name="shoppingListId">The unique identifier of the <see cref="ShoppingList"/>.</param>
	/// <returns>A task representing the asynchronous operation, containing an enumeration of items for the specified list.</returns>
	Task<IEnumerable<ShoppingListItem>> GetAllItemsForShoppingList(int shoppingListId);

	/// <summary>
	/// Adds a product to an active shopping list for its associated store, creating a new list if necessary.
	/// </summary>
	/// <param name="productId">The unique identifier of the <see cref="Product"/> to add.</param>
	/// <param name="quantity">The amount of the product to be added.</param>
	/// <returns>A task representing the asynchronous operation, containing the created <see cref="ShoppingListItem"/>.</returns>
	Task<ShoppingListItem> AddProduct(int productId, int quantity);
}