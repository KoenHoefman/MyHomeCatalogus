using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.Services
{
    public class RoomServiceIntegrationTests : BaseIntegrationTest
    {

        private RoomService CreateRoomService()
        {
            var contextFactory = new DbContextFactoryMock(Options, Context.Database.GetDbConnection());
            return new RoomService(contextFactory);
        }

        #region GetAll

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoRoomsExist()
        {
            var roomService = CreateRoomService();

            var result = await roomService.GetAll();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllRooms()
        {
            await AddTestRoom();

            //Add 2nd room
            Context.Rooms.Add(new Room
            {
                Name = "Foo",
                Description = "Bar"
            });

            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
            Context.ChangeTracker.Clear();

            var roomService = CreateRoomService();

            var result = await roomService.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, r => Assert.IsType<Room>(r));
        }

        #endregion

        #region Get

        [Fact]
        public async Task Get_ShouldThrowKeyNotFoundException_WhenRoomDoesNotExist()
        {
            var nonExistentId = 999;

            var roomService = CreateRoomService();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => roomService.Get(nonExistentId));
        }

        [Fact]
        public async Task Get_ShouldReturnRoom_WhenRoomExists()
        {
            var addedRoom = await AddTestRoom();

            var roomService = CreateRoomService();

            var result = await roomService.Get(addedRoom.Id);

            Assert.NotNull(result);
            Assert.Equal(addedRoom.Id, result.Id);
            Assert.Equal(addedRoom.Name, result.Name);
            Assert.Equal(addedRoom.Description, result.Description);
        }

        #endregion

        #region Add

        [Fact]
        public async Task Add_ShouldThrowArgumentNullException_WhenRoomIsNull()
        {
            var roomService = CreateRoomService();

            await Assert.ThrowsAsync<ArgumentNullException>(() => roomService.Add(null!));
        }

        [Fact]
        public async Task Add_ShouldAddNewRoomAndAssignId()
        {
            var newRoom = new Room
            {
                Name = "Foo", 
                Description = "Bar"
            };

            var roomService = CreateRoomService();

            var addedRoom = await roomService.Add(newRoom);

            Assert.NotNull(addedRoom);

            Assert.True(addedRoom.Id > 0);
            Assert.Equal(newRoom.Name, addedRoom.Name);

            var retrievedRoom = await Context.Rooms.FindAsync([addedRoom.Id],TestContext.Current.CancellationToken);

            Assert.NotNull(retrievedRoom);
            Assert.Equal(addedRoom.Id, retrievedRoom.Id);
        } 

        #endregion

        #region Update

        [Fact]
        public async Task Update_ShouldThrowKeyNotFoundException_WhenRoomDoesNotExist()
        {
            var nonExistentRoom = new Room { Id = 999, Name = "Ghost Room" };

            var roomService = CreateRoomService();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => roomService.Update(nonExistentRoom));
        }

        [Fact]
        public async Task Update_ShouldThrowArgumentNullException_WhenRoomIsNull()
        {
            var roomService = CreateRoomService();

            await Assert.ThrowsAsync<ArgumentNullException>(() => roomService.Update(null!));
        }

        [Fact]
        public async Task Update_ShouldUpdateExistingRoom()
        {
            var newName = "Foo";
            var newDescription = "Bar";

            var roomToUpdate = await AddTestRoom();

            roomToUpdate.Name = newName;
            roomToUpdate.Description = newDescription;

            var roomService = CreateRoomService();

            var updatedRoom = await roomService.Update(roomToUpdate);

            Assert.NotNull(updatedRoom);
            Assert.Equal(roomToUpdate.Id, updatedRoom.Id);
            Assert.Equal(newName, updatedRoom.Name);
            Assert.Equal(newDescription, updatedRoom.Description);

            var retrievedRoom = await Context.Rooms.FindAsync([roomToUpdate.Id], TestContext.Current.CancellationToken);

            Assert.NotNull(retrievedRoom);
            Assert.Equal(newName, retrievedRoom.Name);
            Assert.Equal(newDescription, retrievedRoom.Description);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete_ShouldDoNothing_WhenRoomDoesNotExist()
        {
            var nonExistentId = 999;

            var roomService = CreateRoomService();

            var ex = await Record.ExceptionAsync(() => roomService.Delete(nonExistentId));

            Assert.Null(ex);
        }

        [Fact]
        public async Task Delete_ShouldRemoveRoomFromDatabase()
        {
            var roomToDelete = await AddTestRoom();

            var roomService = CreateRoomService();

            await roomService.Delete(roomToDelete.Id);

            var deletedRoom = await Context.Rooms.FindAsync([roomToDelete.Id], TestContext.Current.CancellationToken);

            Assert.Null(deletedRoom);
        }

        #endregion

        #region ValidateItem

        [Fact]
        public async Task ValidateItem_ShouldReturnEmpty_WhenRoomNameIsUnique()
        {
            var room = new Room { Name = "Foo" };

            var service = CreateRoomService();

            var errors = await service.ValidateItem(room);

            Assert.Empty(errors);
        }

        [Fact]
        public async Task ValidateItem_ShouldReturnError_WhenRoomNameAlreadyExists()
        {
            var existingRoom = await AddTestRoom();

            var newRoom = new Room
            {
                Name = existingRoom.Name
            };

            var service = CreateRoomService();

            var errors = await service.ValidateItem(newRoom);

            var error = Assert.Single(errors);
            Assert.Equal(nameof(ProductType.Name), error.PropertyName);
            Assert.Equal("A room with this name already exists.", error.ErrorMessage);
        }

        [Fact]
        public async Task ValidateItem_ShouldNotReturnError_WhenUpdatingExistingRoomWithSameName()
        {
            var existingRoom = await AddTestRoom();

            var service = CreateRoomService();

            var errors = await service.ValidateItem(existingRoom);

            Assert.Empty(errors);
        }

        #endregion
    }
}
