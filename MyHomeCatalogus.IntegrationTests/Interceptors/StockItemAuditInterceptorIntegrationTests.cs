using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.Interceptors
{
	public class StockItemAuditInterceptorIntegrationTests : BaseIntegrationTest
	{
		[Fact]
		public async Task SavingChanges_When_StockItem_Is_Added_Creates_AuditRecord()
		{
			var testStockItem = await AddTestStockItem();

			var savedStockItem = await Context.StockItems.FindAsync([testStockItem.Id], cancellationToken: TestContext.Current.CancellationToken);

			Assert.NotNull(savedStockItem);
			Assert.Equal(testStockItem.Quantity, savedStockItem.Quantity);
			Assert.Equal(testStockItem.ProductId, savedStockItem.ProductId);
			Assert.Equal(testStockItem.ShelfId, savedStockItem.ShelfId);
			Assert.Equal(testStockItem.StockUnitId, savedStockItem.StockUnitId);

			var audit = await Context.StockItemAudits
				.SingleAsync(a => a.StockItemId == savedStockItem.Id, TestContext.Current.CancellationToken);

			Assert.Equal(savedStockItem.Id, audit.StockItemId);
			Assert.Equal(0, audit.OldQuantity);
			Assert.Equal(savedStockItem.Quantity, audit.NewQuantity);
		}

		[Fact]
		public async Task SavingChanges_When_StockItem_Is_Modified_Creates_AuditRecord()
		{
			var testStockItem = await AddTestStockItem();

			var savedStockItem = await Context.StockItems.FindAsync([testStockItem.Id], cancellationToken: TestContext.Current.CancellationToken);

			Assert.NotNull(savedStockItem);
			Assert.Equal(testStockItem.Quantity, savedStockItem.Quantity);
			Assert.Equal(testStockItem.ProductId, savedStockItem.ProductId);
			Assert.Equal(testStockItem.ShelfId, savedStockItem.ShelfId);
			Assert.Equal(testStockItem.StockUnitId, savedStockItem.StockUnitId);

			var updatedQuantity = testStockItem.Quantity + 10;

			savedStockItem.Quantity = updatedQuantity;

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

			var auditRecords = await Context.StockItemAudits
				.Where(a => a.StockItemId == savedStockItem.Id)
				.OrderBy(a => a.AuditDate)
				.ToListAsync(cancellationToken: TestContext.Current.CancellationToken);

			Assert.Equal(2, auditRecords.Count);

			// Last audit record should reflect the modification
			var audit = auditRecords.Last();

			Assert.Equal(testStockItem.Quantity, audit.OldQuantity);
			Assert.Equal(updatedQuantity, audit.NewQuantity);
		}


		[Fact]
		public async Task SavingChanges_Should_Be_Able_To_Handle_multiple_Audits()
		{
			var testStockItem = await AddTestStockItem();

			var initalAudit = await Context.StockItemAudits
				.SingleAsync(a => a.StockItemId == testStockItem.Id, TestContext.Current.CancellationToken);

			var secondProduct = Context.Products.Add(new Product
			{
				Name = "Bar",
				ProductTypeId = (await Context.ProductTypes.FirstAsync(TestContext.Current.CancellationToken)).Id,
				StoreId = (await Context.Stores.FirstAsync(TestContext.Current.CancellationToken)).Id,
				PurchaseUnitId = (await Context.PurchaseUnits.FirstAsync(TestContext.Current.CancellationToken)).Id
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			//Update 1st stockitem
			var firstStockItem = await Context.StockItems.FirstAsync(TestContext.Current.CancellationToken);

			firstStockItem.Quantity += 10;

			//Add new stockitem
			var secondStockItem = new StockItem
			{
				ProductId = secondProduct.Entity.Id,
				ShelfId = firstStockItem.ShelfId,
				StockUnitId = firstStockItem.StockUnitId,
				Quantity = 25
			};

			Context.StockItems.Add(secondStockItem);

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

			//Get auditRecords but exclude the initial one
			var auditRecords = await Context.StockItemAudits
				.Where(a => (a.StockItemId == firstStockItem.Id && a.Id != initalAudit.Id)
							|| a.StockItemId == secondStockItem.Id)
				.ToListAsync(cancellationToken: TestContext.Current.CancellationToken);

			Assert.Equal(2, auditRecords.Count);

			//Updated item
			var firstAudit = auditRecords.Single(a => a.StockItemId == firstStockItem.Id);

			Assert.Equal(testStockItem.Quantity, firstAudit.OldQuantity);
			Assert.Equal(firstStockItem.Quantity, firstAudit.NewQuantity);

			//Added item
			var secondAudit = auditRecords.Single(a => a.StockItemId == secondStockItem.Id);

			Assert.Equal(0, secondAudit.OldQuantity);
			Assert.Equal(secondStockItem.Quantity, secondAudit.NewQuantity);
		}


	}
}
