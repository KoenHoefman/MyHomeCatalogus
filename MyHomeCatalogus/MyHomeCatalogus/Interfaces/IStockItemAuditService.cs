using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Interfaces;

/// <summary>
/// Defines operations for retrieving historical audit records related to <see cref="StockItem"/> changes.
/// </summary>
public interface IStockItemAuditService
{
    /// <summary>
    /// Retrieves a chronological history of all quantity changes for a specific stock item.
    /// </summary>
    /// <param name="stockItemId">The unique identifier of the <see cref="StockItem"/> to audit.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains an enumeration 
    /// of <see cref="StockItemAudit"/> records associated with the specified item.
    /// </returns>
    Task<IEnumerable<StockItemAudit>> GetAuditsByStockItemId(int stockItemId);
}