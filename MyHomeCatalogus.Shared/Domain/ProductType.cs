using MyHomeCatalogus.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MyHomeCatalogus.Shared.Domain;

/// <summary>
/// Represents a category or classification for products (e.g., "Dairy", "Vegetables", "Electronics").
/// </summary>
public class ProductType : IEntity
{
    /// <inheritdoc />
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the product type.
    /// </summary>
    /// <remarks>
    /// Required field. Maximum length: 50 characters.
    /// </remarks>
    [Required]
    [StringLength(50, ErrorMessage = "Name is too long.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a description providing more context about this product category.
    /// </summary>
    /// <remarks>
    /// Optional field. Maximum length: 500 characters.
    /// </remarks>
    [StringLength(500, ErrorMessage = "Description is too long.")]
    public string? Description { get; set; }
}