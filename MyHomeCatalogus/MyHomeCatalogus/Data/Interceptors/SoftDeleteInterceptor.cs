using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using MyHomeCatalogus.Shared.Interfaces;

namespace MyHomeCatalogus.Data.Interceptors
{
	public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
	{
		private readonly ILogger<StockItemAuditInterceptor> _logger;

		public SoftDeleteInterceptor(ILogger<StockItemAuditInterceptor>? logger = null)
		{
			_logger = logger ?? NullLogger<StockItemAuditInterceptor>.Instance;
		}

		//ToDo: Wait for this until the end.
		//This will give issues regarding relations that need to be solved
		public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,
			InterceptionResult<int> result)
		{
			try
			{
				var context = eventData.Context;
				if (context == null)
				{
					return result;
				}

				DoSoftDelete(context);
				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error executing soft delete during SavingChanges.");
				throw;
			}
		}

		public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
			DbContextEventData eventData, InterceptionResult<int> result,
			CancellationToken cancellationToken = default)
		{
			try
			{
				var context = eventData.Context;
				if (context == null)
				{
					return base.SavingChangesAsync(
					eventData, result, cancellationToken);
				}

				DoSoftDelete(context);

				return base.SavingChangesAsync(eventData, result, cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error executing soft delete during SavingChangesAsync.");
				throw;
			}
		}

		private void DoSoftDelete(DbContext context)
		{
			var entries = context
					   .ChangeTracker
					   .Entries<ISoftDeletable>()
					   .Where(e => e.State == EntityState.Deleted)
					   .ToList();

			foreach (var entry in entries)
			{
				entry.State = EntityState.Modified;
				entry.Entity.IsDeleted = true;
				entry.Entity.DateDeleted = DateTime.Now;

				_logger.LogDebug("SoftDelete executed for entity {EntityType} with id {EntityId}", entry.Entity.GetType().ToString(), ((IEntity)entry.Entity).Id);
			}
		}
	}
}
