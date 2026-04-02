using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.Services
{
	public class StockItemAuditServiceIntegrationTests : BaseIntegrationTest
	{

		private StockItemAuditService CreateStockItemAuditService()
		{
			var contextFactory = new DbContextFactoryMock(Options, Context.Database.GetDbConnection());
			return new StockItemAuditService(contextFactory, NullLogger<StockItemAuditService>.Instance);
		}

		#region GetAuditsByStockItemId

		[Fact]
		public async Task GetAuditsByStockItemId_ShouldReturnEmptyList_WhenNoAuditsExistForId()
		{
			var testStockItem = await AddTestStockItem();

			//Clear audits because above line adds an initial audit-record due to the interceptor
			await Context.StockItemAudits.ExecuteDeleteAsync(TestContext.Current.CancellationToken);

			Assert.Empty(await Context.StockItemAudits.ToListAsync(TestContext.Current.CancellationToken));

			var auditService = CreateStockItemAuditService();

			var result = await auditService.GetAuditsByStockItemId(testStockItem.Id);

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAuditsByStockItemId_ShouldReturnEmptyList_WhenStockItemDoesNotExist()
		{
			var nonExistentId = 9999;

			var auditService = CreateStockItemAuditService();

			var result = await auditService.GetAuditsByStockItemId(nonExistentId);

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAuditsByStockItemId_ShouldReturnAudits_WhenMatchingIdExists()
		{
			//Add multiple stockItems to ensure filtering works

			var testProduct = await AddTestProduct();
			var testStockUnit = await AddTestStockUnit();
			var testStorageUnit = await AddTestStorageUnit();

			var firstTestShelf = Context.Shelves.Add(new Shelf()
			{
				Name = "First shelf",
				Description = "Foo",
				StorageUnitId = testStorageUnit.Id
			});

			var secondTestShelf = Context.Shelves.Add(new Shelf()
			{
				Name = "Second shelf",
				Description = "Foo",
				StorageUnitId = testStorageUnit.Id
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var firstStockItem = Context.StockItems.Add(new StockItem()
			{
				ProductId = testProduct.Id,
				ShelfId = firstTestShelf.Entity.Id,
				StockUnitId = testStockUnit.Id,
				Quantity = 50
			});

			var secondStockItem = Context.StockItems.Add(new StockItem()
			{
				ProductId = testProduct.Id,
				ShelfId = secondTestShelf.Entity.Id,
				StockUnitId = testStockUnit.Id,
				Quantity = 50
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			//Update 1 stockItem to add an extra audit
			var addedStockItem = await Context.StockItems.FindAsync([firstStockItem.Entity.Id], TestContext.Current.CancellationToken);
			addedStockItem!.Quantity += 10;

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var auditService = CreateStockItemAuditService();

			var result = await auditService.GetAuditsByStockItemId(firstStockItem.Entity.Id);

			Assert.NotNull(result);
			Assert.Equal(2, result.Count());
			Assert.All(result, a => Assert.Equal(firstStockItem.Entity.Id, a.StockItemId));
		}

		#endregion

	}
}
