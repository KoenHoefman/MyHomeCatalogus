using MyHomeCatalogus.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MyHomeCatalogus.Shared.Domain;

/// <summary>
/// Represents a physical location or area within the home (e.g., "Kitchen", "Basement") used to organize storage.
/// </summary>
public class Room : IEntity
{
	/// <inheritdoc />
	public int Id { get; set; }

	/// <summary>
	/// Gets or sets the name of the room.
	/// </summary>
	/// <remarks>
	/// Required field. Maximum length: 50 characters.
	/// </remarks>
	[Required]
	[StringLength(50, ErrorMessage = "Name is too long.")]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets a description providing additional details about the room or its contents.
	/// </summary>
	/// <remarks>
	/// Optional field. Maximum length: 500 characters.
	/// </remarks>
	[StringLength(500, ErrorMessage = "Description is too long.")]
	public string? Description { get; set; }

	/// <summary>
	/// Gets the collection of storage units (e.g., cabinets, fridges) located within this specific room.
	/// </summary>
	public IEnumerable<StorageUnit> StorageUnits { get; } = new List<StorageUnit>();
}