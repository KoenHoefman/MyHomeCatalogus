using MyHomeCatalogus.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MyHomeCatalogus.Shared.Domain;

/// <summary>
/// Represents the unit of measurement used when purchasing a product (e.g., "Bottle", "Pack", "Kilogram").
/// </summary>
public class PurchaseUnit : IEntity
{
    /// <inheritdoc />
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the purchase unit.
    /// </summary>
    /// <remarks>
    /// Required field. Maximum length: 50 characters.
    /// </remarks>
    [Required]
    [StringLength(50, ErrorMessage = "Name is too long.")]
    public string Name { get; set; } = string.Empty;
}