using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHomeCatalogus.IntegrationTests.Base
{
    public class DbContextFactoryMock(DbContextOptions<AppDbContext> options, DbConnection connection) : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext()
        {
            // Build new options that force the new context to use the existing connection
            var newOptions = new DbContextOptionsBuilder<AppDbContext>(options)
                .UseSqlite(connection) 
                .Options;

            // Create a new context instance with the shared connection
            return new AppDbContext(newOptions);
        }

        public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(CreateDbContext());
        }
    }
}
