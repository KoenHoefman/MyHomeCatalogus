using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
	public class StockUnitIntegrationTests : BaseIntegrationTest
	{
		//UQ_stockunits_name
		[Fact]
		public async Task StockUnit_Name_Cannot_Be_Duplicated()
		{
			var testStockUnit = await AddTestStockUnit();

			var duplicateStockUnit = new StockUnit()
			{
				Name = testStockUnit.Name
			};

			Context.StockUnits.Add(duplicateStockUnit);

			await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
		}

	}
}
