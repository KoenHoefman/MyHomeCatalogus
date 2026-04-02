using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.Services
{
	public class ShoppingListItemServiceIntegrationTests : BaseIntegrationTest
	{

		private ShoppingListItemService CreateShoppingListItemService()
		{
			var contextFactory = new DbContextFactoryMock(Options, Context.Database.GetDbConnection());
			return new ShoppingListItemService(contextFactory, NullLogger<ShoppingListItemService>.Instance);
		}


		#region GetAll

		[Fact]
		public async Task GetAll_ShouldThrowNotImplementedException()
		{
			var shoppingListItemService = CreateShoppingListItemService();

			await Assert.ThrowsAsync<NotImplementedException>(() => shoppingListItemService.GetAll());
		}

		#endregion


		#region GetAllItemsForShoppingList

		[Fact]
		public async Task GetAllItemsForShoppingList_ShouldReturnEmptyList_WhenListDoesNotExist()
		{
			var shoppingListItemService = CreateShoppingListItemService();

			var result = await shoppingListItemService.GetAllItemsForShoppingList(1);

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAllItemsForShoppingList_ShouldReturnEmptyList_WhenNoItemsInList()
		{
			var shoppingList = await AddTestShoppingList();
			var shoppingListItemService = CreateShoppingListItemService();

			var result = await shoppingListItemService.GetAllItemsForShoppingList(shoppingList.Id);

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAllItemsForShoppingList_ShouldReturnAllItems()
		{
			var shoppingListItem = await AddTestShoppingListItem();

			var shoppingListItemService = CreateShoppingListItemService();

			var result = await shoppingListItemService.GetAllItemsForShoppingList(shoppingListItem.ShoppingListId);

			Assert.NotNull(result);
			Assert.Single(result);

			var resultItem = result.Single();

			Assert.Equal(shoppingListItem.Id, resultItem.Id);
			Assert.Equal(shoppingListItem.ProductId, resultItem.ProductId);
			Assert.Equal(shoppingListItem.ShoppingListId, resultItem.ShoppingListId);
			Assert.Equal(shoppingListItem.Quantity, resultItem.Quantity);
		}

		#endregion

		#region Get

		[Fact]
		public async Task Get_ShouldThrowKeyNotFoundException_WhenItemDoesNotExist()
		{
			var nonExistentId = 999;

			var shoppingListItemService = CreateShoppingListItemService();

			await Assert.ThrowsAsync<KeyNotFoundException>(() => shoppingListItemService.Get(nonExistentId));
		}

		[Fact]
		public async Task Get_ShouldReturnItem_WhenItemExists()
		{
			var shoppingListItem = await AddTestShoppingListItem();

			var shoppingListItemService = CreateShoppingListItemService();

			var result = await shoppingListItemService.Get(shoppingListItem.Id);

			Assert.NotNull(result);
			Assert.Equal(shoppingListItem.Id, result.Id);
			Assert.Equal(shoppingListItem.ProductId, result.ProductId);
			Assert.Equal(shoppingListItem.ShoppingListId, result.ShoppingListId);
			Assert.Equal(shoppingListItem.Quantity, result.Quantity);
		}

		#endregion

		#region Add

		[Fact]
		public async Task Add_ShouldThrowArgumentNullException_WhenShoppingListItemIsNull()
		{
			var shoppingListItemService = CreateShoppingListItemService();

			await Assert.ThrowsAsync<ArgumentNullException>(() => shoppingListItemService.Add(null!));
		}

		[Fact]
		public async Task Add_ShouldAddNewItemAndAssignId()
		{
			var testProduct = await AddTestProduct();

			var testShoppingList = Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = testProduct.StoreId,
				IsCompleted = false,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var newItem = new ShoppingListItem
			{
				ShoppingListId = testShoppingList.Entity.Id,
				ProductId = testProduct.Id,
				Quantity = 5
			};

			var shoppingListItemService = CreateShoppingListItemService();

			var addedItem = await shoppingListItemService.Add(newItem);

			Assert.NotNull(addedItem);

			Assert.True(addedItem.Id > 0);
			Assert.Equal(newItem.ShoppingListId, addedItem.ShoppingListId);
			Assert.Equal(newItem.ProductId, addedItem.ProductId);
			Assert.Equal(newItem.Quantity, addedItem.Quantity);

			var retrievedItem = await Context.ShoppingListItems.FindAsync([addedItem.Id], TestContext.Current.CancellationToken);

			Assert.NotNull(retrievedItem);
			Assert.Equal(addedItem.Id, retrievedItem.Id);
			Assert.Equal(addedItem.ShoppingListId, retrievedItem.ShoppingListId);
			Assert.Equal(addedItem.ProductId, retrievedItem.ProductId);
			Assert.Equal(addedItem.Quantity, retrievedItem.Quantity);
		}

		#endregion

		#region Update

		[Fact]
		public async Task Update_ShouldThrowKeyNotFoundException_WhenItemDoesNotExist()
		{
			var testProduct = await AddTestProduct();

			var testShoppingList = Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = testProduct.StoreId,
				IsCompleted = false,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var nonExistentItem = new ShoppingListItem()
			{
				Id = 999,
				ProductId = testProduct.Id,
				ShoppingListId = testShoppingList.Entity.Id,
				Quantity = 5
			};

			var shoppingListItemService = CreateShoppingListItemService();

			await Assert.ThrowsAsync<KeyNotFoundException>(() => shoppingListItemService.Update(nonExistentItem));
		}

		[Fact]
		public async Task Update_ShouldThrowArgumentNullException_WhenItemIsNull()
		{
			var shoppingListItemService = CreateShoppingListItemService();

			await Assert.ThrowsAsync<ArgumentNullException>(() => shoppingListItemService.Update(null!));
		}

		[Fact]
		public async Task Update_ShouldUpdateExistingItem()
		{
			var oldQuantity = 10;
			var newQuantity = 20;

			var testProduct = await AddTestProduct();

			var testShoppingList = Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = testProduct.StoreId,
				IsCompleted = false,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			});

			var newStore = Context.Stores.Add(new Store
			{
				Name = "New store",
				Description = "The new store"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var newProduct = Context.Products.Add(new Product
			{
				Name = "Foo",
				ProductTypeId = testProduct.ProductTypeId,
				StoreId = newStore.Entity.Id,
				PurchaseUnitId = testProduct.PurchaseUnitId
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var newList = Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = newStore.Entity.Id,
				IsCompleted = false,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			});

			var originalItem = Context.ShoppingListItems.Add(new ShoppingListItem
			{
				ProductId = testProduct.Id,
				ShoppingListId = testShoppingList.Entity.Id,
				Quantity = oldQuantity
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var itemToUpdate = originalItem.Entity;

			itemToUpdate.ProductId = newProduct.Entity.Id;
			itemToUpdate.ShoppingListId = newList.Entity.Id;
			itemToUpdate.Quantity = newQuantity;

			var shoppingListItemService = CreateShoppingListItemService();

			var updatedItem = await shoppingListItemService.Update(itemToUpdate);

			Assert.NotNull(updatedItem);
			Assert.Equal(itemToUpdate.Id, updatedItem.Id);
			Assert.Equal(itemToUpdate.ProductId, updatedItem.ProductId);
			Assert.Equal(itemToUpdate.ShoppingListId, updatedItem.ShoppingListId);
			Assert.Equal(itemToUpdate.Quantity, updatedItem.Quantity);

			var retrievedItem = await Context.ShoppingListItems.FindAsync([itemToUpdate.Id], TestContext.Current.CancellationToken);

			Assert.NotNull(retrievedItem);
			Assert.Equal(itemToUpdate.Id, retrievedItem.Id);
			Assert.Equal(itemToUpdate.ProductId, retrievedItem.ProductId);
			Assert.Equal(itemToUpdate.ShoppingListId, retrievedItem.ShoppingListId);
			Assert.Equal(itemToUpdate.Quantity, retrievedItem.Quantity);
		}

		#endregion

		#region Delete

		[Fact]
		public async Task Delete_ShouldDoNothing_WhenItemDoesNotExist()
		{
			var nonExistentId = 999;

			var shoppingListItemService = CreateShoppingListItemService();

			var ex = await Record.ExceptionAsync(() => shoppingListItemService.Delete(nonExistentId));

			// Assert that no exception was thrown and the method simply returned
			Assert.Null(ex);
		}

		[Fact]
		public async Task Delete_ShouldRemoveItemFromDatabase()
		{
			var itemToDelete = await AddTestShoppingListItem();

			var shoppingListItemService = CreateShoppingListItemService();

			await shoppingListItemService.Delete(itemToDelete.Id);

			var deletedItem = await Context.ShoppingListItems.FindAsync([itemToDelete.Id], TestContext.Current.CancellationToken);

			Assert.Null(deletedItem);
		}

		#endregion

		#region ValidateItem

		[Fact]
		public async Task ValidateItem_ShouldReturnError_WhenQuantityIsLessThanOne()
		{
			var product = await AddTestProduct();
			var shoppingList = Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = product.StoreId,
				IsCompleted = false
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var item = new ShoppingListItem
			{
				ProductId = product.Id,
				ShoppingListId = shoppingList.Entity.Id,
				Quantity = 0
			};

			var service = CreateShoppingListItemService();

			var errors = await service.ValidateItem(item);

			var error = Assert.Single(errors);
			Assert.Equal(nameof(ShoppingListItem.Quantity), error.PropertyName);
			Assert.Equal("Quantity must be greater than 0.", error.ErrorMessage);
		}

		[Fact]
		public async Task ValidateItem_ShouldReturnError_WhenProductAlreadyInList()
		{
			var testItem = await AddTestShoppingListItem();

			var duplicateItem = new ShoppingListItem
			{
				ShoppingListId = testItem.ShoppingListId,
				ProductId = testItem.ProductId,
				Quantity = 100
			};

			var shoppingListItemService = CreateShoppingListItemService();

			var results = await shoppingListItemService.ValidateItem(duplicateItem);

			var error = Assert.Single(results);
			Assert.Equal(nameof(ShoppingListItem.ProductId), error.PropertyName);
			Assert.Equal("This product is already on the shoppinglist.", error.ErrorMessage);
		}

		[Fact]
		public async Task ValidateItem_ShouldReturnError_WhenStoreMismatchBetweenProductAndList()
		{
			var testProduct = await AddTestProduct();

			var otherStore = Context.Stores.Add(new Store
			{
				Name = "Foo"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var testShoppingList = Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = otherStore.Entity.Id,
				IsCompleted = false
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var itemWithWrongStore = new ShoppingListItem
			{
				ShoppingListId = testShoppingList.Entity.Id,
				ProductId = testProduct.Id,
				Quantity = 1
			};

			var shoppingListItemService = CreateShoppingListItemService();

			var result = await shoppingListItemService.ValidateItem(itemWithWrongStore);

			var error = Assert.Single(result);
			Assert.Equal(nameof(ShoppingListItem.ShoppingListId), error.PropertyName);
			Assert.Equal("The product is from a different store compared to the shoppinglist.", error.ErrorMessage);
		}

		[Fact]
		public async Task ValidateItem_ShouldReturnEmpty_WhenItemIsValid()
		{
			var testProduct = await AddTestProduct();

			var testList = Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = testProduct.StoreId,
				IsCompleted = false,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			});

			var newProduct = Context.Products.Add(new Product
			{
				Name = "Foo",
				ProductTypeId = testProduct.ProductTypeId,
				StoreId = testProduct.StoreId,
				PurchaseUnitId = testProduct.PurchaseUnitId
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var existingItem = Context.ShoppingListItems.Add(new ShoppingListItem
			{
				ShoppingListId = testList.Entity.Id,
				ProductId = testProduct.Id,
				Quantity = 5
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var newItem = new ShoppingListItem
			{
				ShoppingListId = existingItem.Entity.ShoppingListId,
				ProductId = newProduct.Entity.Id, // Different product
				Quantity = 1
			};

			var shoppingListItemService = CreateShoppingListItemService();

			var results = await shoppingListItemService.ValidateItem(newItem);

			Assert.Empty(results);
		}

		#endregion

	}
}
