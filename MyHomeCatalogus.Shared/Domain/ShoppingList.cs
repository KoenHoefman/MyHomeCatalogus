using MyHomeCatalogus.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MyHomeCatalogus.Shared.Domain;

/// <summary>
/// Represents a collection of products to be purchased from a specific store.
/// </summary>
public class ShoppingList : IEntity
{
	/// <inheritdoc />
	public int Id { get; set; }

	/// <summary>
	/// Gets or sets the foreign key for the associated <see cref="Domain.Store"/>.
	/// </summary>
	/// <remarks>
	/// Required field with a value greater than 0.
	/// </remarks>
	[Range(1, int.MaxValue, ErrorMessage = "Store is required.")]
	public int StoreId { get; set; }

	/// <summary>
	/// Gets or sets the navigation property for the store associated with this shopping list.
	/// </summary>
	public Store? Store { get; set; }

	/// <summary>
	/// Gets or sets the date when the shopping list was initially created.
	/// </summary>
	public DateOnly? DateCreated { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the shopping list has been processed and finalized.
	/// </summary>
	public bool IsCompleted { get; set; }

	/// <summary>
	/// Gets the collection of individual items and quantities included in this shopping list.
	/// </summary>
	public IEnumerable<ShoppingListItem> ShoppingListItems { get; } = new List<ShoppingListItem>();
}