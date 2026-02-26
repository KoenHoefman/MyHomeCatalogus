using MyHomeCatalogus.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MyHomeCatalogus.Shared.Domain;

/// <summary>
/// Represents an individual instruction or step within a recipe, including the ingredients required for that specific step.
/// </summary>
public class RecipePreparationStep : IEntity
{
    /// <inheritdoc />
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the associated <see cref="Domain.Recipe"/>.
    /// </summary>
    /// <remarks>
    /// Required field with a value greater than 0.
    /// </remarks>
    [Range(1, int.MaxValue, ErrorMessage = "Recipe is required.")]
    public int RecipeId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the recipe this step belongs to.
    /// </summary>
    public Recipe? Recipe { get; set; }

    /// <summary>
    /// Gets or sets the sequence number of this step within the recipe.
    /// </summary>
    /// <remarks>
    /// Required field with a value greater than 0. Used for ordering instructions.
    /// </remarks>
    [Range(1, int.MaxValue, ErrorMessage = "Step must be greater than 0.")]
    public int StepNumber { get; set; }

    /// <summary>
    /// Gets or sets the detailed text instructions for this specific preparation step.
    /// </summary>
    /// <remarks>
    /// Required field.
    /// </remarks>
    [Required]
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// Gets the collection of ingredients specifically used during this preparation step.
    /// </summary>
    public IEnumerable<RecipeIngredient> Ingredients { get; } = new List<RecipeIngredient>();
}