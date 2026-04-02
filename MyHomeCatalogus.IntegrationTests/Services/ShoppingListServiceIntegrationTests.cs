using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.Services
{
	public class ShoppingListServiceIntegrationTests : BaseIntegrationTest
	{

		private ShoppingListService CreateShoppingListService()
		{
			var contextFactory = new DbContextFactoryMock(Options, Context.Database.GetDbConnection());
			return new ShoppingListService(contextFactory, NullLogger<ShoppingListService>.Instance);
		}

		#region GetAll

		[Fact]
		public async Task GetAll_ShouldReturnEmptyList_WhenNoShoppingListsExist()
		{
			var shoppingListService = CreateShoppingListService();

			var result = await shoppingListService.GetAll();

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAll_ShouldReturnAllShoppingLists()
		{
			await AddTestShoppingList();

			//Add 2nd ShoppingList
			var secondStore = Context.Stores.Add(new Store
			{
				Name = "Foo",
				Description = "Bar"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = secondStore.Entity.Id,
				IsCompleted = true,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var shoppingListService = CreateShoppingListService();

			var result = await shoppingListService.GetAll();

			Assert.NotNull(result);
			Assert.Equal(2, result.Count());
			Assert.All(result, r => Assert.IsType<ShoppingList>(r));
		}

		#endregion

		#region GetShoppingListById

		[Fact]
		public async Task GetShoppingListById_ShouldThrowKeyNotFoundException_WhenShoppingListDoesNotExist()
		{
			var nonExistentId = 999;

			var shoppingListService = CreateShoppingListService();

			await Assert.ThrowsAsync<KeyNotFoundException>(() => shoppingListService.Get(nonExistentId));
		}

		[Fact]
		public async Task GetShoppingListById_ShouldReturnShoppingList_WhenShoppingListExists()
		{
			var addedShoppingList = await AddTestShoppingList();

			var shoppingListService = CreateShoppingListService();

			var result = await shoppingListService.Get(addedShoppingList.Id);

			Assert.NotNull(result);
			Assert.Equal(addedShoppingList.Id, result.Id);
			Assert.Equal(addedShoppingList.StoreId, result.StoreId);
			Assert.Equal(addedShoppingList.DateCreated, result.DateCreated);
			Assert.Equal(addedShoppingList.IsCompleted, result.IsCompleted);
		}

		#endregion

		#region AddShoppingList

		[Fact]
		public async Task AddShoppingList_ShouldThrowArgumentNullException_WhenShoppingListIsNull()
		{
			var shoppingListService = CreateShoppingListService();

			await Assert.ThrowsAsync<ArgumentNullException>(() => shoppingListService.Add(null!));
		}

		[Fact]
		public async Task AddShoppingList_ShouldAddNewShoppingListAndAssignId()
		{
			var newShoppingList = new ShoppingList
			{
				StoreId = (await AddTestStore()).Id,
				IsCompleted = false,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			};

			var shoppingListService = CreateShoppingListService();

			var addedShoppingList = await shoppingListService.Add(newShoppingList);

			Assert.NotNull(addedShoppingList);

			// 1. Check if the ID was assigned by the database
			Assert.True(addedShoppingList.Id > 0);
			Assert.Equal(newShoppingList.StoreId, addedShoppingList.StoreId);
			Assert.Equal(newShoppingList.DateCreated, addedShoppingList.DateCreated);
			Assert.Equal(newShoppingList.IsCompleted, addedShoppingList.IsCompleted);

			// 2. Verify it was actually saved to the database
			var retrievedShoppingList = await Context.ShoppingLists.FindAsync([addedShoppingList.Id], TestContext.Current.CancellationToken);

			Assert.NotNull(retrievedShoppingList);
			Assert.Equal(addedShoppingList.Id, retrievedShoppingList.Id);
			Assert.Equal(addedShoppingList.StoreId, retrievedShoppingList.StoreId);
			Assert.Equal(addedShoppingList.DateCreated, retrievedShoppingList.DateCreated);
			Assert.Equal(addedShoppingList.IsCompleted, retrievedShoppingList.IsCompleted);
		}

		#endregion

		#region UpdateShoppingList

		[Fact]
		public async Task UpdateShoppingList_ShouldThrowKeyNotFoundException_WhenShoppingListDoesNotExist()
		{
			var nonExistentShoppingList = new ShoppingList
			{
				Id = 999,
				StoreId = (await AddTestStore()).Id,
				IsCompleted = false,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			};

			var shoppingListService = CreateShoppingListService();

			await Assert.ThrowsAsync<KeyNotFoundException>(() => shoppingListService.Update(nonExistentShoppingList));
		}

		[Fact]
		public async Task UpdateShoppingList_ShouldThrowArgumentNullException_WhenShoppingListIsNull()
		{
			var shoppingListService = CreateShoppingListService();

			await Assert.ThrowsAsync<ArgumentNullException>(() => shoppingListService.Update(null!));
		}

		[Fact]
		public async Task Update_ShouldThrowInvalidOperationException_WhenStoreIsChangedWhileHavingItems()
		{
			var shoppingListItem = await AddTestShoppingListItem();

			var newStore = Context.Stores.Add(new Store
			{
				Name = "Foo",
				Description = "Bar"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var shoppingListToUpdate = await Context.ShoppingLists.FirstOrDefaultAsync(s => s.Id == shoppingListItem.ShoppingListId, TestContext.Current.CancellationToken);

			shoppingListToUpdate!.StoreId = newStore.Entity.Id;

			var shoppingListService = CreateShoppingListService();

			var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => shoppingListService.Update(shoppingListToUpdate));

			Assert.NotNull(ex);
			Assert.Equal("Cannot change the store because the shopping list already contains items.", ex.Message);
		}

		[Fact]
		public async Task UpdateShoppingList_ShouldUpdateExistingShoppingList()
		{
			var shoppingListToUpdate = await AddTestShoppingList();

			var newStore = Context.Stores.Add(new Store
			{
				Name = "Foo",
				Description = "Bar"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var newDate = shoppingListToUpdate.DateCreated!.Value.AddDays(-1);
			var newIsCompleted = !shoppingListToUpdate.IsCompleted;


			shoppingListToUpdate.StoreId = newStore.Entity.Id;
			shoppingListToUpdate.DateCreated = newDate;
			shoppingListToUpdate.IsCompleted = newIsCompleted;

			var shoppingListService = CreateShoppingListService();

			var updatedShoppingList = await shoppingListService.Update(shoppingListToUpdate);

			Assert.NotNull(updatedShoppingList);
			Assert.Equal(shoppingListToUpdate.Id, updatedShoppingList.Id);
			Assert.Equal(shoppingListToUpdate.StoreId, updatedShoppingList.StoreId);
			Assert.Equal(shoppingListToUpdate.DateCreated, updatedShoppingList.DateCreated);
			Assert.Equal(shoppingListToUpdate.IsCompleted, updatedShoppingList.IsCompleted);

			// Verify the change was saved to the database
			var retrievedShoppingList = await Context.ShoppingLists.FindAsync([shoppingListToUpdate.Id], TestContext.Current.CancellationToken);

			Assert.NotNull(retrievedShoppingList);
			Assert.Equal(newStore.Entity.Id, retrievedShoppingList.StoreId);
			Assert.Equal(newDate, retrievedShoppingList.DateCreated);
			Assert.Equal(newIsCompleted, retrievedShoppingList.IsCompleted);
		}

		#endregion

		#region DeleteShoppingList

		[Fact]
		public async Task DeleteShoppingList_ShouldDoNothing_WhenShoppingListDoesNotExist()
		{
			var nonExistentId = 999;

			var shoppingListService = CreateShoppingListService();

			var ex = await Record.ExceptionAsync(() => shoppingListService.Delete(nonExistentId));

			// Assert that no exception was thrown and the method simply returned
			Assert.Null(ex);
		}

		[Fact]
		public async Task DeleteShoppingList_ShouldRemoveShoppingListFromDatabase()
		{
			var shoppingListToDelete = await AddTestShoppingList();

			var shoppingListService = CreateShoppingListService();

			await shoppingListService.Delete(shoppingListToDelete.Id);

			var deletedShoppingList = await Context.ShoppingLists.FindAsync([shoppingListToDelete.Id], TestContext.Current.CancellationToken);

			Assert.Null(deletedShoppingList);
		}

		#endregion

		#region GetAllActiveShoppingLists

		[Fact]
		public async Task GetAllActiveShoppingLists_ShouldReturnEmptyList_WhenNoShoppingListsExist()
		{
			var shoppingListService = CreateShoppingListService();

			var result = await shoppingListService.GetAllActiveShoppingLists();

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAllActiveShoppingLists_ShouldReturnEmptyList_WhenNoActiveShoppingListsExist()
		{
			var shoppingList = await AddTestShoppingList();

			var shoppingListToUpdate = await Context.ShoppingLists.FindAsync([shoppingList.Id], TestContext.Current.CancellationToken);

			shoppingListToUpdate!.IsCompleted = true;

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var shoppingListService = CreateShoppingListService();

			var result = await shoppingListService.GetAllActiveShoppingLists();

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAllActiveShoppingLists_ShouldReturnAllActiveShoppingLists()
		{
			await AddTestShoppingList();

			//Add 2nd ShoppingList
			var secondStore = Context.Stores.Add(new Store
			{
				Name = "Foo",
				Description = "Bar"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = secondStore.Entity.Id,
				IsCompleted = false,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var shoppingListService = CreateShoppingListService();

			var result = await shoppingListService.GetAllActiveShoppingLists();

			Assert.NotNull(result);
			Assert.Equal(2, result.Count());
			Assert.All(result, r => Assert.IsType<ShoppingList>(r));
		}

		#endregion

		#region GetItemsCountForActiveShoppingLists

		[Fact]
		public async Task GetItemsCountForActiveShoppingLists_FiltersOutCompletedLists()
		{
			#region Arrange

			var testProduct1 = await AddTestProduct();

			//AddTestShoppingList uses same AddTestStore which gives UQ index violation
			var list1Active = Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = testProduct1.StoreId,
				IsCompleted = false,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			});

			var store2 = Context.Stores.Add(new Store { Name = "Second store" });

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var list2Completed = Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = store2.Entity.Id,
				IsCompleted = true,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			});

			var store3 = Context.Stores.Add(new Store { Name = "Third store" });

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var list3Active = Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = store3.Entity.Id,
				IsCompleted = false,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			#endregion
			var shoppingListService = CreateShoppingListService();

			var result = (await shoppingListService.GetItemsCountForActiveShoppingLists()).ToList();

			Assert.NotNull(result);
			Assert.Equal(2, result.Count);
			Assert.Contains(result, l => l.ShoppingListId == list1Active.Entity.Id);
			Assert.Contains(result, l => l.ShoppingListId == list3Active.Entity.Id);
			Assert.DoesNotContain(result, l => l.ShoppingListId == list2Completed.Entity.Id);
		}

		[Fact]
		public async Task GetItemsCountForActiveShoppingLists_CalculatesItemCountsCorrectly()
		{
			var listOneItem = await AddTestShoppingList();

			var store2 = Context.Stores.Add(new Store { Name = "Second store" });

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var listThreeItems = Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = store2.Entity.Id,
				IsCompleted = false,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			});

			var testProduct1 = Context.Products.Add(new Product
			{
				Name = "Product 1",
				ProductTypeId = (await AddTestProductType()).Id,
				StoreId = listOneItem.StoreId,
				PurchaseUnitId = (await AddTestPurchaseUnit()).Id
			});

			var testProduct31 = Context.Products.Add(new Product
			{
				Name = "Product 31",
				ProductTypeId = testProduct1.Entity.ProductTypeId,
				StoreId = store2.Entity.Id,
				PurchaseUnitId = testProduct1.Entity.PurchaseUnitId
			});

			var testProduct32 = Context.Products.Add(new Product
			{
				Name = "Product 32",
				ProductTypeId = testProduct1.Entity.ProductTypeId,
				StoreId = store2.Entity.Id,
				PurchaseUnitId = testProduct1.Entity.PurchaseUnitId
			});

			var testProduct33 = Context.Products.Add(new Product
			{
				Name = "Product 33",
				ProductTypeId = testProduct1.Entity.ProductTypeId,
				StoreId = store2.Entity.Id,
				PurchaseUnitId = testProduct1.Entity.PurchaseUnitId
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var item1 = Context.ShoppingListItems.Add(
				new ShoppingListItem
				{
					ProductId = testProduct1.Entity.Id,
					Quantity = 5,
					ShoppingListId = listOneItem.Id
				});

			var item31 = Context.ShoppingListItems.Add(
				new ShoppingListItem
				{
					ProductId = testProduct31.Entity.Id,
					Quantity = 5,
					ShoppingListId = listThreeItems.Entity.Id
				});

			var item32 = Context.ShoppingListItems.Add(
				new ShoppingListItem
				{
					ProductId = testProduct32.Entity.Id,
					Quantity = 5,
					ShoppingListId = listThreeItems.Entity.Id
				});

			var item33 = Context.ShoppingListItems.Add(
				new ShoppingListItem
				{
					ProductId = testProduct33.Entity.Id,
					Quantity = 5,
					ShoppingListId = listThreeItems.Entity.Id
				});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var shoppingListService = CreateShoppingListService();

			var result = (await shoppingListService.GetItemsCountForActiveShoppingLists()).ToList();

			var resultList = result.ToList();
			Assert.Equal(2, resultList.Count);
			Assert.Equal(1, resultList.Single(l => l.ShoppingListId == listOneItem.Id).ItemsCount);
			Assert.Equal(3, resultList.Single(l => l.ShoppingListId == listThreeItems.Entity.Id).ItemsCount);
		}

		[Fact]
		public async Task GetItemsCountForActiveShoppingLists_UsesStoreName()
		{
			var store = await AddTestStore();

			var lst = Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = store.Id,
				IsCompleted = false,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var shoppingListService = CreateShoppingListService();

			var result = await shoppingListService.GetItemsCountForActiveShoppingLists();

			Assert.NotNull(result);
			Assert.Equal(store.Name, result.Single().ShoppingListName);
		}

		[Fact]
		public async Task GetItemsCountForActiveShoppingLists_ReturnsEmptyIfNoActiveLists()
		{
			var shoppingListService = CreateShoppingListService();

			var result = await shoppingListService.GetItemsCountForActiveShoppingLists();

			Assert.Empty(result);
		}

		#endregion

		#region ValidateItem

		[Fact]
		public async Task ValidateItem_ShouldReturnEmpty_WhenNoErrors()
		{
			var testStore = await AddTestStore();

			var newItem = new ShoppingList
			{
				StoreId = testStore.Id,
				IsCompleted = false,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			};

			var shoppingListService = CreateShoppingListService();

			var results = await shoppingListService.ValidateItem(newItem);

			Assert.Empty(results);
		}

		[Fact]
		public async Task ValidateItem_ShouldReturnError_WhenActiveListAlreadyExistsForStore()
		{
			var testList = await AddTestShoppingList();

			var duplicateItem = new ShoppingList
			{
				StoreId = testList.StoreId,
				IsCompleted = false,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			};

			var shoppingListService = CreateShoppingListService();

			var results = await shoppingListService.ValidateItem(duplicateItem);

			var error = Assert.Single(results);
			Assert.Equal(nameof(ShoppingList.StoreId), error.PropertyName);
			Assert.Equal("There is already an active shoppinglist for this store.", error.ErrorMessage);
		}

		[Fact]
		public async Task ValidateItem_ShouldReturnEmpty_WhenExistingListsAreCompleted()
		{
			var store = await AddTestStore();

			Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = store.Id,
				IsCompleted = true,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var newActiveList = new ShoppingList
			{
				StoreId = store.Id,
				IsCompleted = false
			};

			var service = CreateShoppingListService();


			var errors = await service.ValidateItem(newActiveList);

			Assert.Empty(errors);
		}

		[Fact]
		public async Task ValidateItem_ShouldNotReturnError_WhenUpdatingExistingActiveList()
		{
			var existingList = await AddTestShoppingList();

			var service = CreateShoppingListService();

			var errors = await service.ValidateItem(existingList);

			Assert.Empty(errors);
		}

		[Fact]
		public async Task ValidateItem_ShouldAllowMultipleCompletedListsForSameStore()
		{
			var store = await AddTestStore();

			Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = store.Id,
				IsCompleted = true
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var anotherCompletedList = new ShoppingList
			{
				StoreId = store.Id,
				IsCompleted = true
			};

			var service = CreateShoppingListService();

			var errors = await service.ValidateItem(anotherCompletedList);

			Assert.Empty(errors);
		}
		#endregion
	}
}
