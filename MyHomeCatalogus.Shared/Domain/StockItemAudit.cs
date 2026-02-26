using MyHomeCatalogus.Shared.Interfaces;

namespace MyHomeCatalogus.Shared.Domain;

/// <summary>
/// Represents a historical record of changes made to a <see cref="Domain.StockItem"/>'s quantity.
/// </summary>
public class StockItemAudit : IEntity
{
    /// <inheritdoc />
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the associated <see cref="Domain.StockItem"/>.
    /// </summary>
    public int StockItemId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the stock item being audited.
    /// </summary>
    public StockItem? StockItem { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the stock change occurred.
    /// </summary>
    public DateTime? AuditDate { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the item before the update.
    /// </summary>
    public int OldQuantity { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the item after the update.
    /// </summary>
    public int NewQuantity { get; set; }
}