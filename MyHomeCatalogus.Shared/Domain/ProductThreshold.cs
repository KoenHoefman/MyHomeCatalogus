using MyHomeCatalogus.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MyHomeCatalogus.Shared.Domain;

/// <summary>
/// Represents the inventory threshold settings for a specific product to manage automated restocking.
/// </summary>
public class ProductThreshold : IEntity
{
	/// <inheritdoc />
	public int Id { get; set; }

	/// <summary>
	/// Gets or sets the foreign key for the associated <see cref="Domain.Product"/>.
	/// </summary>
	/// <remarks>
	/// Required field with a value greater than 0.
	/// </remarks>
	[Range(1, int.MaxValue, ErrorMessage = "Product is required.")]
	public int ProductId { get; set; }

	/// <summary>
	/// Gets or sets the navigation property for the product this threshold applies to.
	/// </summary>
	public Product? Product { get; set; }

	/// <summary>
	/// Gets or sets the minimum quantity that should be in stock. 
	/// </summary>
	/// <remarks>
	/// When the current stock equals or falls below this value, the product is considered for the shopping list.
	/// Must be 0 or greater.
	/// </remarks>
	[Range(0, int.MaxValue, ErrorMessage = "Threshold must be 0 or greater.")]
	public int Threshold { get; set; }

	/// <summary>
	/// Gets or sets the default quantity to be added to the shopping list when the threshold is reached.
	/// </summary>
	/// <remarks>
	/// Required field with a value greater than 0.
	/// </remarks>
	[Range(1, int.MaxValue, ErrorMessage = "PurchaseQuantity must be greater than 0.")]
	public int PurchaseQuantity { get; set; }
}