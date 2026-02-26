using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Interfaces;

/// <summary>
/// Defines specialized data operations for <see cref="Store"/> entities, extending the base data service capabilities.
/// </summary>
public interface IStoreService : IDataService<Store>
{
}