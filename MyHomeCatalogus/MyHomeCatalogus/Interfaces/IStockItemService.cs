using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Interfaces;

/// <summary>
/// Defines specialized data operations for <see cref="StockItem"/> entities, extending the base data service capabilities.
/// </summary>
public interface IStockItemService: IDataService<StockItem>
{
}