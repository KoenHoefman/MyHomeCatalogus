using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Interfaces;

/// <summary>
/// Defines specialized data operations for <see cref="Shelf"/> entities, extending the base data service capabilities.
/// </summary>
public interface IShelfService:IDataService<Shelf>
{
}