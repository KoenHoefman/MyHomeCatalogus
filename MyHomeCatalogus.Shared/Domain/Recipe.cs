using MyHomeCatalogus.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MyHomeCatalogus.Shared.Domain;

/// <summary>
/// Represents a culinary recipe containing a name, description, and a sequence of preparation steps.
/// </summary>
public class Recipe : IEntity
{
    /// <inheritdoc />
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the recipe (e.g., "Spaghetti Carbonara").
    /// </summary>
    /// <remarks>
    /// Required field. Maximum length: 50 characters.
    /// </remarks>
    [Required]
    [StringLength(50, ErrorMessage = "Name is too long.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a brief summary or background information about the recipe.
    /// </summary>
    /// <remarks>
    /// Optional field. Maximum length: 500 characters.
    /// </remarks>
    [StringLength(500, ErrorMessage = "Description is too long.")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets the collection of ordered preparation steps required to complete the recipe.
    /// </summary>
    public IEnumerable<RecipePreparationStep> PreparationSteps { get; } = new List<RecipePreparationStep>();
}