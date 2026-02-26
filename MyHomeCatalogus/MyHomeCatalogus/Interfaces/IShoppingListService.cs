using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Interfaces;

/// <summary>
/// Defines specialized operations for managing <see cref="ShoppingList"/> entities, specifically focusing on active lists and dashboard summaries.
/// </summary>
public interface IShoppingListService : IDataService<ShoppingList>
{
    /// <summary>
    /// Retrieves all shopping lists that are currently marked as not completed.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, containing an enumeration of active <see cref="ShoppingList"/> entities.</returns>
    Task<IEnumerable<ShoppingList>> GetAllActiveShoppingLists();

    /// <summary>
    /// Retrieves a summarized view of all active shopping lists, including store names and item counts, optimized for UI widgets.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, containing an enumeration of <see cref="ShoppingListWidgetData"/> objects.</returns>
    Task<IEnumerable<ShoppingListWidgetData>> GetItemsCountForActiveShoppingLists();
}