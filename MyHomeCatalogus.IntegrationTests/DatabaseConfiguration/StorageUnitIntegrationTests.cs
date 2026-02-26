using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
    public class StorageUnitIntegrationTests : BaseIntegrationTest
    {
        //UQ_storageunits_name_room_id
        [Fact]
        public async Task StorageUnit_Cannot_Have_Same_Name_In_Same_Room()
        {
            var testStorageUnit = await AddTestStorageUnit();

            var duplicateStorageUnit = new StorageUnit
            {
                Name = testStorageUnit.Name,
                RoomId = testStorageUnit.RoomId,
                Description = "Duplicate"
            };

            Context.StorageUnits.Add(duplicateStorageUnit);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        //UQ_storageunits_name_room_id
        [Fact]
        public async Task StorageUnit_Allows_Same_Name_In_Different_Room()
        {
            var testStorageUnit = await AddTestStorageUnit();

            var otherRoom = Context.Rooms.Add(new Room
            {
                Name = "Basement",
                Description = "The basement"
            });

            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
            Context.ChangeTracker.Clear();

            var uniqueStorageUnit = new StorageUnit { Name = "Fridge", RoomId = 2, Description = "Unique" };
            Context.StorageUnits.Add(uniqueStorageUnit);

            var result = await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            Assert.Equal(1, result);
        }

        //FK_shelves_storageunits
        [Fact]
        public async Task StorageUnit_With_Linked_Shelves_Will_Delete_Shelves()
        {
            var testShelf = await AddTestShelf();

            var storageUnitToDelete = await Context.StorageUnits.FindAsync([testShelf.StorageUnitId], TestContext.Current.CancellationToken);

            if (storageUnitToDelete != null)
            {
                Context.StorageUnits.Remove(storageUnitToDelete);
            }

            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var deletedUnit = await Context.StorageUnits.FindAsync([storageUnitToDelete!.Id], TestContext.Current.CancellationToken);
            var deletedShelf = await Context.Shelves.FindAsync([testShelf.Id], TestContext.Current.CancellationToken);

            Assert.Null(deletedUnit);
            Assert.Null(deletedShelf);
        }

        [Fact]
        public async Task StorageUnit_AutoInclude_NaviagationProperties()
        {
            var addedEntity = await AddTestShelf();

            var result = await Context.StorageUnits
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == addedEntity.StorageUnitId, TestContext.Current.CancellationToken);

            Assert.NotNull(result);

            //AutoInclude
            Assert.NotNull(result.Room);
            Assert.NotNull(result.Shelves);
            Assert.NotEmpty(result.Shelves);
        }

    }
}
