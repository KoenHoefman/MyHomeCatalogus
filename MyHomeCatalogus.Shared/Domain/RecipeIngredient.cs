using MyHomeCatalogus.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MyHomeCatalogus.Shared.Domain;

/// <summary>
/// Represents a specific product and quantity required for a particular recipe preparation step.
/// </summary>
public class RecipeIngredient : IEntity
{
	/// <inheritdoc />
	public int Id { get; set; }

	/// <summary>
	/// Gets or sets the foreign key for the associated <see cref="Domain.RecipePreparationStep"/>.
	/// </summary>
	/// <remarks>
	/// Required field with a value greater than 0.
	/// </remarks>
	[Range(1, int.MaxValue, ErrorMessage = "RecipePreparationStep is required.")]
	public int RecipePreparationStepId { get; set; }

	/// <summary>
	/// Gets or sets the navigation property for the preparation step this ingredient belongs to.
	/// </summary>
	public RecipePreparationStep? RecipePreparationStep { get; set; }

	/// <summary>
	/// Gets or sets the foreign key for the associated <see cref="Domain.Product"/>.
	/// </summary>
	/// <remarks>
	/// Required field with a value greater than 0.
	/// </remarks>
	[Range(1, int.MaxValue, ErrorMessage = "Product is required.")]
	public int ProductId { get; set; }

	/// <summary>
	/// Gets or sets the navigation property for the product used as an ingredient.
	/// </summary>
	public Product? Product { get; set; }

	/// <summary>
	/// Gets or sets the amount of the product required for the recipe.
	/// </summary>
	/// <remarks>
	/// Required field with a value greater than 0.
	/// </remarks>
	[Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
	public int Quantity { get; set; }

	/// <summary>
	/// Gets or sets the foreign key for the associated <see cref="Domain.RecipeIngredientMeasurement"/>.
	/// </summary>
	/// <remarks>
	/// Required field with a value greater than 0.
	/// </remarks>
	[Range(1, int.MaxValue, ErrorMessage = "RecipeIngredientMeasurement is required.")]
	public int RecipeIngredientMeasurementId { get; set; }

	/// <summary>
	/// Gets or sets the navigation property defining the unit of measurement (e.g., "Grams", "Teaspoon") for this ingredient.
	/// </summary>
	public RecipeIngredientMeasurement? RecipeIngredientMeasurement { get; set; }
}