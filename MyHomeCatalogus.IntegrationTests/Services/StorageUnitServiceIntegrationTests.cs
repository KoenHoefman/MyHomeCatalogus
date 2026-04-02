using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.Services
{
	public class StorageUnitServiceIntegrationTests : BaseIntegrationTest
	{

		private StorageUnitService CreateStorageUnitService()
		{
			var contextFactory = new DbContextFactoryMock(Options, Context.Database.GetDbConnection());
			return new StorageUnitService(contextFactory, NullLogger<StorageUnitService>.Instance);
		}

		#region GetAll

		[Fact]
		public async Task GetAll_ShouldReturnEmptyList_WhenNoStorageUnitsExist()
		{
			var storageUnitService = CreateStorageUnitService();

			var result = await storageUnitService.GetAll();

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAll_ShouldReturnAllStorageUnits()
		{
			var testStorageUnit = await AddTestStorageUnit();

			//Add 2nd StorageUnit
			Context.StorageUnits.Add(new StorageUnit
			{
				Name = "Foo",
				Description = "Bar",
				RoomId = testStorageUnit.RoomId
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var storageUnitService = CreateStorageUnitService();

			var result = await storageUnitService.GetAll();

			Assert.NotNull(result);
			Assert.Equal(2, result.Count());
			Assert.All(result, r => Assert.IsType<StorageUnit>(r));
		}

		#endregion

		#region Get

		[Fact]
		public async Task Get_ShouldThrowKeyNotFoundException_WhenStorageUnitDoesNotExist()
		{
			var nonExistentId = 999;

			var storageUnitService = CreateStorageUnitService();

			await Assert.ThrowsAsync<KeyNotFoundException>(() => storageUnitService.Get(nonExistentId));
		}

		[Fact]
		public async Task Get_ShouldReturnStorageUnit_WhenStorageUnitExists()
		{
			var addedStorageUnit = await AddTestStorageUnit();

			var storageUnitService = CreateStorageUnitService();

			var result = await storageUnitService.Get(addedStorageUnit.Id);

			Assert.NotNull(result);
			Assert.Equal(addedStorageUnit.Id, result.Id);
			Assert.Equal(addedStorageUnit.Name, result.Name);
			Assert.Equal(addedStorageUnit.Description, result.Description);
		}

		#endregion

		#region Add

		[Fact]
		public async Task Add_ShouldThrowArgumentNullException_WhenStorageUnitIsNull()
		{
			var storageUnitService = CreateStorageUnitService();

			await Assert.ThrowsAsync<ArgumentNullException>(() => storageUnitService.Add(null!));
		}

		[Fact]
		public async Task Add_ShouldAddNewStorageUnitAndAssignId()
		{
			var testRoom = await AddTestRoom();

			var newStorageUnit = new StorageUnit
			{
				Name = "Foo",
				Description = "Bar",
				RoomId = testRoom.Id
			};

			var storageUnitService = CreateStorageUnitService();

			var addedStorageUnit = await storageUnitService.Add(newStorageUnit);

			Assert.NotNull(addedStorageUnit);

			Assert.True(addedStorageUnit.Id > 0);
			Assert.Equal(newStorageUnit.Name, addedStorageUnit.Name);

			var retrievedStorageUnit = await Context.StorageUnits.FindAsync([addedStorageUnit.Id], TestContext.Current.CancellationToken);

			Assert.NotNull(retrievedStorageUnit);
			Assert.Equal(addedStorageUnit.Id, retrievedStorageUnit.Id);
		}

		#endregion

		#region Update

		[Fact]
		public async Task Update_ShouldThrowKeyNotFoundException_WhenStorageUnitDoesNotExist()
		{
			var nonExistentStorageUnit = new StorageUnit { Id = 999, Name = "Ghost StorageUnit" };

			var storageUnitService = CreateStorageUnitService();

			await Assert.ThrowsAsync<KeyNotFoundException>(() => storageUnitService.Update(nonExistentStorageUnit));
		}

		[Fact]
		public async Task Update_ShouldThrowArgumentNullException_WhenStorageUnitIsNull()
		{
			var storageUnitService = CreateStorageUnitService();

			await Assert.ThrowsAsync<ArgumentNullException>(() => storageUnitService.Update(null!));
		}

		[Fact]
		public async Task Update_ShouldUpdateExistingStorageUnit()
		{
			var newName = "Foo";
			var newDescription = "Bar";

			var storageUnitToUpdate = await AddTestStorageUnit();

			storageUnitToUpdate.Name = newName;
			storageUnitToUpdate.Description = newDescription;

			var storageUnitService = CreateStorageUnitService();

			var updatedStorageUnit = await storageUnitService.Update(storageUnitToUpdate);

			Assert.NotNull(updatedStorageUnit);
			Assert.Equal(storageUnitToUpdate.Id, updatedStorageUnit.Id);
			Assert.Equal(newName, updatedStorageUnit.Name);
			Assert.Equal(newDescription, updatedStorageUnit.Description);

			var retrievedStorageUnit = await Context.StorageUnits.FindAsync([storageUnitToUpdate.Id], TestContext.Current.CancellationToken);

			Assert.NotNull(retrievedStorageUnit);
			Assert.Equal(newName, retrievedStorageUnit.Name);
			Assert.Equal(newDescription, retrievedStorageUnit.Description);
		}

		#endregion

		#region Delete

		[Fact]
		public async Task Delete_ShouldDoNothing_WhenStorageUnitDoesNotExist()
		{
			var nonExistentId = 999;

			var storageUnitService = CreateStorageUnitService();

			var ex = await Record.ExceptionAsync(() => storageUnitService.Delete(nonExistentId));

			// Assert that no exception was thrown and the method simply returned
			Assert.Null(ex);
		}

		[Fact]
		public async Task Delete_ShouldRemoveStorageUnitFromDatabase()
		{
			var storageUnitToDelete = await AddTestStorageUnit();

			var storageUnitService = CreateStorageUnitService();

			await storageUnitService.Delete(storageUnitToDelete.Id);

			var deletedStorageUnit = await Context.StorageUnits.FindAsync([storageUnitToDelete.Id], TestContext.Current.CancellationToken);

			Assert.Null(deletedStorageUnit);
		}

		#endregion

		#region ValidateItem

		[Fact]
		public async Task ValidateItem_ShouldReturnEmpty_WhenStorageUnitNameIsUniqueInRoom()
		{
			var room = await AddTestRoom();

			var storageUnit = new StorageUnit() { Name = "Foo", RoomId = room.Id };
			var service = CreateStorageUnitService();

			var errors = await service.ValidateItem(storageUnit);

			Assert.Empty(errors);
		}

		[Fact]
		public async Task ValidateItem_ShouldReturnError_WhenStorageUnitNameAlreadyExistsInSameRoom()
		{
			var existingStorageUnit = await AddTestStorageUnit();

			var duplicateUnit = new StorageUnit
			{
				Name = existingStorageUnit.Name,
				RoomId = existingStorageUnit.RoomId
			};

			var service = CreateStorageUnitService();

			var errors = await service.ValidateItem(duplicateUnit);

			var error = Assert.Single(errors);
			Assert.Equal(nameof(StorageUnit.Name), error.PropertyName);
			Assert.Equal("A storage unit with this name already exists in this room.", error.ErrorMessage);
		}

		[Fact]
		public async Task ValidateItem_ShouldAllowSameStorageUnitNameInDifferentRooms()
		{
			var sharedName = "Bar";
			var roomA = await AddTestRoom();

			var roomB = Context.Rooms.Add(new Room
			{
				Name = "Foo"
			});

			Context.StorageUnits.Add(new StorageUnit
			{
				Name = sharedName,
				RoomId = roomA.Id
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var unitForRoomB = new StorageUnit
			{
				Name = sharedName,
				RoomId = roomB.Entity.Id
			};
			var service = CreateStorageUnitService();

			var errors = await service.ValidateItem(unitForRoomB);

			Assert.Empty(errors);
		}

		[Fact]
		public async Task ValidateItem_ShouldNotReturnError_WhenUpdatingExistingStorageUnitWithSameName()
		{
			var existingStorageUnit = await AddTestStorageUnit();

			var service = CreateStorageUnitService();

			var errors = await service.ValidateItem(existingStorageUnit);

			Assert.Empty(errors);
		}

		#endregion

	}
}
