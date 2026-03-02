using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
	public class ShelfIntegrationTests : BaseIntegrationTest
	{
		//UQ_shelves_name_storageunit_id
		[Fact]
		public async Task Shelves_In_Same_StorageUnit_Must_Have_Unique_Name()
		{
			var testShelf = await AddTestShelf();

			var duplicateShelf = new Shelf()
			{
				Name = testShelf.Name,
				StorageUnitId = testShelf.StorageUnitId
			};

			Context.Shelves.Add(duplicateShelf);

			await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
		}

		//UQ_shelves_name_storageunit_id
		[Fact]
		public async Task Shelves_In_Different_StorageUnitCan_Have_Same_Name()
		{
			var testShelf = await AddTestShelf();

			var testStorageUnit = await Context.StorageUnits.FindAsync([testShelf.StorageUnitId], TestContext.Current.CancellationToken);

			var otherStorageUnit = Context.StorageUnits.Add(new StorageUnit()
			{
				Name = "Other unit",
				Description = "Foo",
				RoomId = testStorageUnit!.RoomId
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var nonDuplicateShelf = new Shelf()
			{
				Name = testShelf.Name,
				StorageUnitId = otherStorageUnit.Entity.Id
			};

			Context.Shelves.Add(nonDuplicateShelf);

			var result = await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

			Assert.Equal(1, result);
		}


		[Fact]
		public async Task Shelf_AutoInclude_NaviagationProperties()
		{
			var addedEntity = await AddTestShelf();

			var result = await Context.Shelves
				.AsNoTracking()
				.FirstOrDefaultAsync(p => p.Id == addedEntity.Id, TestContext.Current.CancellationToken);

			Assert.NotNull(result);

			//AutoInclude
			Assert.NotNull(result.StorageUnit);

			//Not included
			Assert.NotNull(result.StockItems);
			Assert.Empty(result.StockItems);
		}

	}
}
