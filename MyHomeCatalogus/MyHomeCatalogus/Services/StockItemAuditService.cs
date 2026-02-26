using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Services
{
    /// <summary>
    /// Provides data management services for <see cref="Store"/> entities.
    /// </summary>
    public class StockItemAuditService : IStockItemAuditService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockItemAuditService"/> class.
        /// </summary>
        /// <param name="contextFactory">The factory used to create <see cref="AppDbContext"/> instances.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contextFactory"/> is null.</exception>
        public StockItemAuditService(IDbContextFactory<AppDbContext> contextFactory)
        {
            ArgumentNullException.ThrowIfNull(contextFactory);

            _contextFactory = contextFactory;
        }

        /// <inheritdoc />>
        public async Task<IEnumerable<StockItemAudit>> GetAuditsByStockItemId(int stockItemId)
        {
            //ToDo: Add sorting / paging
            await using var context = await _contextFactory.CreateDbContextAsync();

            return await context.StockItemAudits.Where(a => a.StockItemId == stockItemId).ToListAsync();
        }

    }
}
