using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
	public class StockItemIntegrationTests : BaseIntegrationTest
	{
		//CK_stockitems_quantity_zero_or_positive
		[Fact]
		public async Task StockItem_Quantity_Cannot_Be_Negative()
		{
			var invalidItem = new StockItem
			{
				ProductId = (await AddTestProduct()).Id,
				ShelfId = (await AddTestShelf()).Id,
				StockUnitId = (await AddTestStockUnit()).Id,
				Quantity = -1
			};

			Context.StockItems.Add(invalidItem);

			await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
		}

		//CK_stockitems_quantity_zero_or_positive
		[Fact]
		public async Task StockItem_Quantity_Can_Be_Zero()
		{
			var validItem = new StockItem
			{
				ProductId = (await AddTestProduct()).Id,
				ShelfId = (await AddTestShelf()).Id,
				StockUnitId = (await AddTestStockUnit()).Id,
				Quantity = 0
			};

			Context.StockItems.Add(validItem);

			var result = await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

			Assert.Equal(1, result);
		}

		[Fact]
		public async Task StockItem_AutoInclude_NaviagationProperties()
		{
			var addedEntity = await AddTestStockItem();

			var result = await Context.StockItems
				.AsNoTracking()
				.FirstOrDefaultAsync(p => p.Id == addedEntity.Id, TestContext.Current.CancellationToken);

			Assert.NotNull(result);

			//AutoInclude
			Assert.NotNull(result.Product);
			Assert.NotNull(result.StockUnit);
			Assert.NotNull(result.Shelf);
		}


	}
}
