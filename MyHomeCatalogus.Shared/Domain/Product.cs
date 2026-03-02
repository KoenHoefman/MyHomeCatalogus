using MyHomeCatalogus.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MyHomeCatalogus.Shared.Domain;

/// <summary>
/// Represents a product item within the catalog, containing metadata, visual data, and association IDs.
/// </summary>
public class Product : IEntity
{
	/// <inheritdoc />
	public int Id { get; set; }

	/// <summary>
	/// Gets or sets the name of the product.
	/// </summary>
	/// <remarks>
	/// Required field. Maximum length: 50 characters.
	/// </remarks>
	[Required]
	[StringLength(50, ErrorMessage = "Name is too long.")]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets a detailed description of the product.
	/// </summary>
	/// <remarks>
	/// Optional field. Maximum length: 500 characters.
	/// </remarks>
	[StringLength(500, ErrorMessage = "Description is too long.")]
	public string? Description { get; set; }

	/// <summary>
	/// Gets or sets the binary data for the product's image.
	/// </summary>
	public byte[]? Picture { get; set; }

	/// <summary>
	/// Gets or sets the MIME type of the product image (e.g., image/jpeg, image/png).
	/// </summary>
	public string? PictureMimeType { get; set; }

	/// <summary>
	/// Gets or sets the binary representation of the product's barcode.
	/// </summary>
	public byte[]? Barcode { get; set; }

	/// <summary>
	/// Gets or sets the MIME type of the barcode image.
	/// </summary>
	public string? BarcodeMimeType { get; set; }

	/// <summary>
	/// Gets or sets the foreign key for the associated <see cref="Domain.ProductType"/>.
	/// </summary>
	/// <remarks>
	/// Required field with a value greater than 0.
	/// </remarks>
	[Range(1, int.MaxValue, ErrorMessage = "ProductType is required.")]
	public int ProductTypeId { get; set; }

	/// <summary>
	/// Gets or sets the navigation property for the product's category or type.
	/// </summary>
	public ProductType? ProductType { get; set; }

	/// <summary>
	/// Gets or sets the foreign key for the associated <see cref="Domain.Store"/>.
	/// </summary>
	/// <remarks>
	/// Required field with a value greater than 0.
	/// </remarks>
	[Range(1, int.MaxValue, ErrorMessage = "Store is required.")]
	public int StoreId { get; set; }

	/// <summary>
	/// Gets or sets the navigation property for the primary store where this product is purchased.
	/// </summary>
	public Store? Store { get; set; }

	/// <summary>
	/// Gets or sets the foreign key for the associated <see cref="Domain.PurchaseUnit"/>.
	/// </summary>
	/// <remarks>
	/// Required field with a value greater than 0.
	/// </remarks>
	[Range(1, int.MaxValue, ErrorMessage = "PurchaseUnit is required")]
	public int PurchaseUnitId { get; set; }

	/// <summary>
	/// Gets or sets the navigation property for the unit in which the product is bought (e.g., Piece, Pack).
	/// </summary>
	public PurchaseUnit? PurchaseUnit { get; set; }

	/// <summary>
	/// Gets the collection of stock items associated with this product across different storage locations.
	/// </summary>
	public IEnumerable<StockItem> StockItems { get; } = new List<StockItem>();
}