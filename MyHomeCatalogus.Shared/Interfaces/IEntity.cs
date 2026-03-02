namespace MyHomeCatalogus.Shared.Interfaces;

/// <summary>
/// Defines the base contract for all domain entities that require a unique identifier.
/// </summary>
public interface IEntity
{
	/// <summary>
	/// Gets or sets the unique identifier for the entity.
	/// </summary>
	int Id { get; set; }
}