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
		private readonly ILogger<StockItemAuditService> _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="StockItemAuditService"/> class.
		/// </summary>
		/// <param name="contextFactory">The factory used to create <see cref="AppDbContext"/> instances.</param>
	/// <param name="logger">The logger for logging errors.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="contextFactory"/> is null.</exception>
		public StockItemAuditService(IDbContextFactory<AppDbContext> contextFactory, ILogger<StockItemAuditService> logger)
		{
			ArgumentNullException.ThrowIfNull(contextFactory);
			ArgumentNullException.ThrowIfNull(logger);

			_contextFactory = contextFactory;
			_logger = logger;
		}

		/// <inheritdoc />>
		public async Task<IEnumerable<StockItemAudit>> GetAuditsByStockItemId(int stockItemId)
		{
			try
			{
				//ToDo: Add sorting / paging
				await using var context = await _contextFactory.CreateDbContextAsync();

				return await context.StockItemAudits.Where(a => a.StockItemId == stockItemId).ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retreiving audits for stockitem {StockItemId}.", stockItemId);
				throw;
			}
		}

	}
}
