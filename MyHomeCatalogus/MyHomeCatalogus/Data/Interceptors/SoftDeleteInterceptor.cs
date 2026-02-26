using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Interfaces;

namespace MyHomeCatalogus.Data.Interceptors
{
    public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        //ToDo: Wait for this until the end.
        //This will give issues regarding relations that need to be solved
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            var context = eventData.Context;
            if (context == null)
            {
                return result;
            }

            DoSoftDelete(context);
            return result;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
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
            }
        }
    }
}
