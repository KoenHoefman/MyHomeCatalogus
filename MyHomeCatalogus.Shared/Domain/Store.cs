using MyHomeCatalogus.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MyHomeCatalogus.Shared.Domain;

/// <summary>
/// Represents a retail location or provider (e.g., "Colruyt", "Bol.com", "Local farmer") where products are purchased.
/// </summary>
public class Store : IEntity
{
	/// <inheritdoc />
	public int Id { get; set; }

	/// <summary>
	/// Gets or sets the name of the store.
	/// </summary>
	/// <remarks>
	/// Required field. Maximum length: 50 characters.
	/// </remarks>
	[Required]
	[StringLength(50, ErrorMessage = "Name is too long.")]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets additional details about the store, such as location or typical inventory.
	/// </summary>
	/// <remarks>
	/// Optional field. Maximum length: 500 characters.
	/// </remarks>
	[StringLength(500, ErrorMessage = "Description is too long.")]
	public string? Description { get; set; }
}