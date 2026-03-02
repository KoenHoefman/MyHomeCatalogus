using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
	public class StockItemAuditIntegrationTests : BaseIntegrationTest
	{
		//CK_stockitem_audits_quantity_old_zero_or_positive
		[Fact]
		public async Task StockItemAudit_QuantityOld_Cannot_Be_Negative()
		{
			var testStockItem = await AddTestStockItem();

			var invalidItem = new StockItemAudit()
			{
				StockItemId = testStockItem.Id,
				OldQuantity = -1,
				NewQuantity = 1,
				AuditDate = DateTime.Now
			};

			Context.StockItemAudits.Add(invalidItem);

			await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
		}

		//CK_stockitem_audits_quantity_old_zero_or_positive
		[Fact]
		public async Task StockItemAudit_QuantityOld_Can_Be_Zero()
		{
			var testStockItem = await AddTestStockItem();

			var validItem = new StockItemAudit
			{
				StockItemId = testStockItem.Id,
				OldQuantity = 0,
				NewQuantity = 1,
				AuditDate = DateTime.Now
			};

			Context.StockItemAudits.Add(validItem);

			var result = await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

			Assert.Equal(1, result);
		}

		//CK_stockitem_audits_quantity_new_zero_or_positive
		[Fact]
		public async Task StockItemAudit_QuantityNew_Cannot_Be_Negative()
		{
			var testStockItem = await AddTestStockItem();

			var invalidItem = new StockItemAudit()
			{
				StockItemId = testStockItem.Id,
				OldQuantity = 1,
				NewQuantity = -1,
				AuditDate = DateTime.Now
			};

			Context.StockItemAudits.Add(invalidItem);

			await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
		}

		//CK_stockitem_audits_quantity_new_zero_or_positive
		[Fact]
		public async Task StockItemAudit_QuantityNew_Can_Be_Zero()
		{
			var testStockItem = await AddTestStockItem();

			var validItem = new StockItemAudit
			{
				StockItemId = testStockItem.Id,
				OldQuantity = 1,
				NewQuantity = 0,
				AuditDate = DateTime.Now
			};

			Context.StockItemAudits.Add(validItem);

			var result = await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

			Assert.Equal(1, result);
		}

		//FK_stockitem_audits_stockitems
		[Fact]
		public async Task Deleteing_Audit_With_Linked_StockItem_Will_Not_Delete_StockItem()
		{
			var testStockItem = await AddTestStockItem();

			var auditToDelete = await Context.StockItemAudits
				.SingleAsync(a => a.StockItemId == testStockItem.Id, TestContext.Current.CancellationToken);

			Context.StockItemAudits.Remove(auditToDelete);

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

			var deletedAudit = await Context.StockItemAudits.FindAsync([auditToDelete.Id], TestContext.Current.CancellationToken);
			Assert.Null(deletedAudit);

			var existingStockItem = await Context.StockItems.FindAsync([testStockItem.Id], TestContext.Current.CancellationToken);
			Assert.NotNull(existingStockItem);
		}

		[Fact]
		public async Task StockItemAudit_AutoInclude_NaviagationProperties()
		{
			var testStockItem = await AddTestStockItem();

			var testAudit = Context.StockItemAudits.Add(new StockItemAudit
			{
				StockItemId = testStockItem.Id,
				OldQuantity = 1,
				NewQuantity = 0,
				AuditDate = DateTime.Now
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var result = await Context.StockItemAudits
				.AsNoTracking()
				.FirstOrDefaultAsync(p => p.Id == testAudit.Entity.Id, TestContext.Current.CancellationToken);

			Assert.NotNull(result);

			//AutoInclude

			//Not included
			Assert.Null(result.StockItem);
		}

	}
}
