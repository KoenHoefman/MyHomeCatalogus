using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Interceptors
{
	public sealed class StockItemAuditInterceptor : SaveChangesInterceptor
	{
		private readonly ILogger<StockItemAuditInterceptor> _logger;
		private readonly List<StockItemAudit> _pendingAudits = new List<StockItemAudit>();

		public StockItemAuditInterceptor(ILogger<StockItemAuditInterceptor>? logger = null)
		{
			_logger = logger ?? NullLogger<StockItemAuditInterceptor>.Instance;
		}

		public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,
		InterceptionResult<int> result)
		{
			try
			{
				var context = eventData.Context;

				if (context is not null)
				{
					CreateAudits(context);
				}

				return result;

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating audits during SavingChanges.");
				throw;
			}
		}

		public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
		DbContextEventData eventData, InterceptionResult<int> result,
		CancellationToken cancellationToken = default)
		{
			try
			{
				var context = eventData.Context;

				if (context is not null)
				{
					CreateAudits(context);
				}

				return await base.SavingChangesAsync(eventData, result, cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating audits during SavingChangesAsync.");
				throw;
			}
		}

		public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
		{
			try
			{
				var context = eventData.Context;
				if (context is not null)
				{
					var auditCount = ProcessPendingAudits(context);

					if (auditCount > 0)
					{
						return base.SavedChanges(eventData, result + context.SaveChanges());
					}
				}

				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing audits during SavedChanges.");
				throw;
			}
		}

		public override async ValueTask<int> SavedChangesAsync(
		SaveChangesCompletedEventData eventData, int result,
		CancellationToken cancellationToken = default)
		{
			try
			{
				var context = eventData.Context;
				if (context == null) return result;

				var auditCount = ProcessPendingAudits(context);

				if (auditCount > 0)
				{
					return result + await context.SaveChangesAsync(cancellationToken);
				}

				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing audits during SavedChangesAsync.");
				throw;
			}
		}

		private void CreateAudits(DbContext context)
		{
			var entries = context.ChangeTracker.Entries().ToList();

			foreach (var entry in entries)
			{
				if (!(entry.Entity is StockItem stockItem)) continue;

				if (entry.State == EntityState.Added)
				{
					if (stockItem.Quantity > 0)
					{
						var stockItemAudit = new StockItemAudit
						{
							StockItem = stockItem,
							OldQuantity = 0,
							NewQuantity = stockItem.Quantity,
							AuditDate = DateTime.Now
						};

						_pendingAudits.Add(stockItemAudit);
						_logger.LogInformation("StockItemAuditInterceptor queued audit for added StockItem with temporary quantity {Quantity}.", stockItem.Quantity);
					}
				}
				else if (entry.State == EntityState.Modified)
				{
					var quantityEntry = entry.Property(nameof(StockItem.Quantity));

					if (quantityEntry.IsModified)
					{
						var stockItemAudit = new StockItemAudit
						{
							StockItemId = stockItem.Id,
							OldQuantity = (int)(quantityEntry.OriginalValue ?? 0),
							NewQuantity = stockItem.Quantity,
							AuditDate = DateTime.Now
						};

						context.Set<StockItemAudit>().Add(stockItemAudit);
						_logger.LogInformation("StockItemAuditInterceptor created audit for modified StockItem {StockItemId}: {OldQuantity} -> {NewQuantity}.", stockItem.Id, stockItemAudit.OldQuantity, stockItemAudit.NewQuantity);
					}
				}
			}
		}

		private int ProcessPendingAudits(DbContext context)
		{
			var count = 0;

			foreach (var audit in _pendingAudits)
			{
				audit.StockItemId = audit.StockItem!.Id;
				audit.StockItem = null;

				context.Set<StockItemAudit>().Add(audit);
				count++;
				_logger.LogInformation("StockItemAuditInterceptor persisting pending audit for StockItemId {StockItemId}.", audit.StockItemId);
			}

			_pendingAudits.Clear();
			_logger.LogDebug("StockItemAuditInterceptor persisted {PersistedCount} pending audit(s).", count);
			return count;
		}
	}
}
