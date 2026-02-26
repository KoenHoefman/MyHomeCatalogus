using MyHomeCatalogus.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MyHomeCatalogus.Shared.Domain;

/// <summary>
/// Represents a unit of measurement for recipe ingredients (e.g., "Gram", "Milliliter", "Tablespoon").
/// </summary>
public class RecipeIngredientMeasurement : IEntity
{
    /// <inheritdoc />
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the full name of the measurement unit.
    /// </summary>
    /// <remarks>
    /// Required field. Maximum length: 50 characters.
    /// </remarks>
    [Required]
    [StringLength(50, ErrorMessage = "Name is too long.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the short-form representation of the measurement (e.g., "g", "ml", "tbsp").
    /// </summary>
    /// <remarks>
    /// Required field. Maximum length: 10 characters.
    /// </remarks>
    [Required]
    [StringLength(10, ErrorMessage = "Abbreviation is too long.")]
    public string Abbreviation { get; set; } = string.Empty;
}