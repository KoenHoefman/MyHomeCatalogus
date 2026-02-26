using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
    public class RoomIntegrationTests : BaseIntegrationTest
    {
        //UQ_rooms_name
        [Fact]
        public async Task Room_Name_Cannot_Be_Duplicated()
        {
            var testRoom = await AddTestRoom();

            var duplicateRoom = new Room
            {
                Name = testRoom.Name, 
                Description = "Dupl room"
            };

            Context.Rooms.Add(duplicateRoom);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        //FK_storageunits_rooms
        [Fact]
        public async Task Room_With_Linked_StorageUnits_Will_Delete_StorageUnits()
        {
            var testStorageUnit = await AddTestStorageUnit();

            var roomToDelete = await Context.Rooms.FindAsync([testStorageUnit.RoomId],TestContext.Current.CancellationToken);

            if (roomToDelete != null)
            {
                Context.Rooms.Remove(roomToDelete);
            }

            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var deletedRoom = await Context.Rooms.FindAsync([roomToDelete!.Id], TestContext.Current.CancellationToken);
            var deletedUnit = await Context.StorageUnits.FindAsync([testStorageUnit.Id],TestContext.Current.CancellationToken);

            Assert.Null(deletedRoom);
            Assert.Null(deletedUnit);
        }

        [Fact]
        public async Task Room_AutoInclude_NaviagationProperties()
        {
            var addedEntity = await AddTestStorageUnit();

            var result = await Context.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == addedEntity.RoomId, TestContext.Current.CancellationToken);

            Assert.NotNull(result);

            //AutoInclude
            Assert.NotNull(result.StorageUnits);
            Assert.NotEmpty(result.StorageUnits);
        }

    }
}
