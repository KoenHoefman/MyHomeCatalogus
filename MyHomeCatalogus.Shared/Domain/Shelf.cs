using MyHomeCatalogus.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MyHomeCatalogus.Shared.Domain;

/// <summary>
/// Represents a specific shelf or subdivision within a <see cref="Domain.StorageUnit"/>.
/// </summary>
public class Shelf : IEntity
{
    /// <inheritdoc />
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the shelf (e.g., "Top Shelf", "Drawer 1").
    /// </summary>
    /// <remarks>
    /// Required field. Maximum length: 50 characters.
    /// </remarks>
    [Required]
    [StringLength(50, ErrorMessage = "Name is too long.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a description providing additional details about the shelf's location or purpose.
    /// </summary>
    /// <remarks>
    /// Optional field. Maximum length: 500 characters.
    /// </remarks>
    [StringLength(500, ErrorMessage = "Description is too long.")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the associated <see cref="Domain.StorageUnit"/>.
    /// </summary>
    /// <remarks>
    /// Required field with a value greater than 0.
    /// </remarks>
    [Range(1, int.MaxValue, ErrorMessage = "StorageUnit is required.")]
    public int StorageUnitId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the storage unit where this shelf is located.
    /// </summary>
    public StorageUnit? StorageUnit { get; set; }

    /// <summary>
    /// Gets the collection of stock items currently stored on this specific shelf.
    /// </summary>
    public IEnumerable<StockItem> StockItems { get; } = new List<StockItem>();
}