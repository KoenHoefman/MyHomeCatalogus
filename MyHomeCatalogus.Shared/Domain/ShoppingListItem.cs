using MyHomeCatalogus.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MyHomeCatalogus.Shared.Domain;

/// <summary>
/// Represents an entry in a <see cref="Domain.ShoppingList"/>, specifying a product and the quantity to be purchased.
/// </summary>
public class ShoppingListItem : IEntity
{
    /// <inheritdoc />
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the parent <see cref="Domain.ShoppingList"/>.
    /// </summary>
    /// <remarks>
    /// Required field with a value greater than 0.
    /// </remarks>
    [Range(1, int.MaxValue, ErrorMessage = "ShoppingList is required.")]
    public int ShoppingListId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the shopping list this item belongs to.
    /// </summary>
    public ShoppingList? ShoppingList { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the associated <see cref="Domain.Product"/>.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the product to be purchased.
    /// </summary>
    public Product? Product { get; set; }

    /// <summary>
    /// Gets or sets the number of units of the product to buy.
    /// </summary>
    /// <remarks>
    /// Required field with a value greater than 0.
    /// </remarks>
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public int Quantity { get; set; }
}