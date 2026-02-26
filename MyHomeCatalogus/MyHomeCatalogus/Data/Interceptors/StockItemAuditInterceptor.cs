using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Interceptors
{
    public sealed class StockItemAuditInterceptor : SaveChangesInterceptor
    {

        private readonly List<StockItemAudit> _pendingAudits = new List<StockItemAudit>();

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            var context = eventData.Context;
            
            if (context is not null)
            {
                CreateAudits(context);
            }

            return result;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;

            if (context is not null)
            {
                CreateAudits(context);
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
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

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData, int result,
            CancellationToken cancellationToken = default)
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

        private void CreateAudits(DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries().ToList())
            {

                if (!(entry.Entity is StockItem stockItem)) continue;

                if (entry.State == EntityState.Added)
                {
                    if (stockItem.Quantity > 0)
                    {
                        //Id of the new StockItem isn't known yet.
                        //Store the object in the audit and save the audits afterwards
                        var stockItemAudit = new StockItemAudit
                        {
                            StockItem = stockItem,
                            OldQuantity = 0,
                            NewQuantity = stockItem.Quantity,
                            AuditDate = DateTime.Now
                        };

                        _pendingAudits.Add(stockItemAudit);
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
            }

            _pendingAudits.Clear();
            return count;
        }

    }
}
