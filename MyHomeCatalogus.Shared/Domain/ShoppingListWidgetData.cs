namespace MyHomeCatalogus.Shared.Domain;

/// <summary>
/// A lightweight data structure used to display shopping list summary information within a UI widget or dashboard.
/// </summary>
/// <param name="shoppingList">The <see cref="ShoppingList"/> entity to extract widget data from.</param>
public class ShoppingListWidgetData(ShoppingList shoppingList)
{
	/// <summary>
	/// Gets or sets the unique identifier of the shopping list.
	/// </summary>
	public int ShoppingListId { get; set; } = shoppingList.Id;

	/// <summary>
	/// Gets or sets the display name for the shopping list, typically derived from the store name.
	/// </summary>
	/// <remarks>
	/// Defaults to "Unknown store" if the associated <see cref="Store"/> navigation property is null.
	/// </remarks>
	public string ShoppingListName { get; set; } = shoppingList.Store?.Name ?? "Unknown store";

	/// <summary>
	/// Gets or sets the total number of unique product entries in the shopping list.
	/// </summary>
	public int ItemsCount { get; set; } = shoppingList.ShoppingListItems.Count();
}