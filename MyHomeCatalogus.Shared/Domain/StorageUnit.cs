using MyHomeCatalogus.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MyHomeCatalogus.Shared.Domain;

/// <summary>
/// Represents a physical storage structure (e.g., "Fridge", "Pantry Cabinet", "Wardrobe") located within a specific <see cref="Room"/>.
/// </summary>
public class StorageUnit : IEntity
{
	/// <inheritdoc />
	public int Id { get; set; }

	/// <summary>
	/// Gets or sets the name of the storage unit.
	/// </summary>
	/// <remarks>
	/// Required field. Maximum length: 50 characters.
	/// </remarks>
	[Required]
	[StringLength(50, ErrorMessage = "Name is too long.")]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets a description or notes about the storage unit.
	/// </summary>
	/// <remarks>
	/// Optional field. Maximum length: 500 characters.
	/// </remarks>
	[StringLength(500, ErrorMessage = "Description is too long.")]
	public string? Description { get; set; }

	/// <summary>
	/// Gets or sets the foreign key for the associated <see cref="Domain.Room"/>.
	/// </summary>
	/// <remarks>
	/// Required field with a value greater than 0.
	/// </remarks>
	[Range(1, int.MaxValue, ErrorMessage = "Room is required.")]
	public int RoomId { get; set; }

	/// <summary>
	/// Gets or sets the navigation property for the room where this storage unit is placed.
	/// </summary>
	public Room? Room { get; set; }

	/// <summary>
	/// Gets the collection of <see cref="Shelf"/> entities that belong to this storage unit.
	/// </summary>
	public IEnumerable<Shelf> Shelves { get; } = new List<Shelf>();
}