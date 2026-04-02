using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.Services
{
	public class StoreServiceIntegrationTests : BaseIntegrationTest
	{

		private StoreService CreateStoreService()
		{
			var contextFactory = new DbContextFactoryMock(Options, Context.Database.GetDbConnection());
			return new StoreService(contextFactory, NullLogger<StoreService>.Instance);
		}

		#region GetAll

		[Fact]
		public async Task GetAll_ShouldReturnEmptyList_WhenNoStoresExist()
		{
			var storeService = CreateStoreService();

			var result = await storeService.GetAll();

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAll_ShouldReturnAllStores()
		{
			await AddTestStore();

			//Add 2nd Store
			Context.Stores.Add(new Store
			{
				Name = "Foo"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var storeService = CreateStoreService();

			var result = await storeService.GetAll();

			Assert.NotNull(result);
			Assert.Equal(2, result.Count());
			Assert.All(result, r => Assert.IsType<Store>(r));
		}

		#endregion

		#region Get

		[Fact]
		public async Task Get_ShouldThrowKeyNotFoundException_WhenStoreDoesNotExist()
		{
			var nonExistentId = 999;

			var storeService = CreateStoreService();

			await Assert.ThrowsAsync<KeyNotFoundException>(() => storeService.Get(nonExistentId));
		}

		[Fact]
		public async Task Get_ShouldReturnStore_WhenStoreExists()
		{
			var addedStore = await AddTestStore();

			var storeService = CreateStoreService();

			var result = await storeService.Get(addedStore.Id);

			Assert.NotNull(result);
			Assert.Equal(addedStore.Id, result.Id);
			Assert.Equal(addedStore.Name, result.Name);
		}

		#endregion

		#region Add

		[Fact]
		public async Task Add_ShouldThrowArgumentNullException_WhenStoreIsNull()
		{
			var storeService = CreateStoreService();

			await Assert.ThrowsAsync<ArgumentNullException>(() => storeService.Add(null!));
		}

		[Fact]
		public async Task Add_ShouldAddNewStoreAndAssignId()
		{
			var newStore = new Store
			{
				Name = "Foo"
			};

			var storeService = CreateStoreService();

			var addedStore = await storeService.Add(newStore);

			Assert.NotNull(addedStore);

			Assert.True(addedStore.Id > 0);
			Assert.Equal(newStore.Name, addedStore.Name);

			var retrievedStore = await Context.Stores.FindAsync([addedStore.Id], TestContext.Current.CancellationToken);

			Assert.NotNull(retrievedStore);
			Assert.Equal(addedStore.Id, retrievedStore.Id);
		}

		#endregion

		#region Update

		[Fact]
		public async Task Update_ShouldThrowKeyNotFoundException_WhenStoreDoesNotExist()
		{
			var nonExistentStore = new Store { Id = 999, Name = "Ghost Store" };

			var storeService = CreateStoreService();

			await Assert.ThrowsAsync<KeyNotFoundException>(() => storeService.Update(nonExistentStore));
		}

		[Fact]
		public async Task Update_ShouldThrowArgumentNullException_WhenStoreIsNull()
		{
			var storeService = CreateStoreService();

			await Assert.ThrowsAsync<ArgumentNullException>(() => storeService.Update(null!));
		}

		[Fact]
		public async Task Update_ShouldUpdateExistingStore()
		{
			var newName = "Foo";

			var storeToUpdate = await AddTestStore();

			storeToUpdate.Name = newName;

			var storeService = CreateStoreService();

			var updatedStore = await storeService.Update(storeToUpdate);

			Assert.NotNull(updatedStore);
			Assert.Equal(storeToUpdate.Id, updatedStore.Id);
			Assert.Equal(newName, updatedStore.Name);

			// Verify the change was saved to the database
			var retrievedStore = await Context.Stores.FindAsync([storeToUpdate.Id], TestContext.Current.CancellationToken);

			Assert.NotNull(retrievedStore);
			Assert.Equal(newName, retrievedStore.Name);
		}

		#endregion

		#region Delete

		[Fact]
		public async Task Delete_ShouldDoNothing_WhenStoreDoesNotExist()
		{
			var nonExistentId = 999;

			var storeService = CreateStoreService();

			var ex = await Record.ExceptionAsync(() => storeService.Delete(nonExistentId));

			Assert.Null(ex);
		}

		[Fact]
		public async Task Delete_ShouldRemoveStoreFromDatabase()
		{
			var storeToDelete = await AddTestStore();

			var storeService = CreateStoreService();

			await storeService.Delete(storeToDelete.Id);

			var deletedStore = await Context.Stores.FindAsync([storeToDelete.Id], TestContext.Current.CancellationToken);

			Assert.Null(deletedStore);
		}

		#endregion

		#region ValidateItem

		[Fact]
		public async Task ValidateItem_ShouldReturnEmpty_WhenStoreNameIsUnique()
		{
			var store = await AddTestStore();

			var service = CreateStoreService();

			var errors = await service.ValidateItem(store);

			Assert.Empty(errors);
		}

		[Fact]
		public async Task ValidateItem_ShouldReturnError_WhenStoreNameAlreadyExists()
		{
			var store = await AddTestStore();

			var newStore = new Store
			{
				Name = store.Name
			};

			var service = CreateStoreService();

			var errors = await service.ValidateItem(newStore);

			var error = Assert.Single(errors);
			Assert.Equal(nameof(Store.Name), error.PropertyName);
			Assert.Equal("A store with this name already exists.", error.ErrorMessage);
		}

		[Fact]
		public async Task ValidateItem_ShouldNotReturnError_WhenUpdatingExistingStoreWithSameName()
		{
			var existingStore = await AddTestStore();

			var service = CreateStoreService();

			var errors = await service.ValidateItem(existingStore);

			Assert.Empty(errors);
		}

		#endregion

	}
}
