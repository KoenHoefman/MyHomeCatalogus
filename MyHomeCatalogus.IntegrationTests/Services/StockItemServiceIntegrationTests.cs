using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.Services
{
    public class StockItemServiceIntegrationTests : BaseIntegrationTest
    {

        private StockItemService CreateStockItemService()
        {
            var contextFactory = new DbContextFactoryMock(Options, Context.Database.GetDbConnection());
            return new StockItemService(contextFactory);
        }

        #region GetAll

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoStockItemsExist()
        {
            var stockItemService = CreateStockItemService();

            var result = await stockItemService.GetAll();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllStockItems()
        {
            var testProduct = await AddTestProduct();
            var testStockUnit = await AddTestStockUnit();
            var testStorageUnit = await AddTestStorageUnit();

            var firstTestShelf = Context.Shelves.Add(new Shelf()
            {
                Name = "First shelf",
                Description = "Foo",
                StorageUnitId = testStorageUnit.Id
            });

            var secondTestShelf = Context.Shelves.Add(new Shelf()
            {
                Name = "Second shelf",
                Description = "Foo",
                StorageUnitId = testStorageUnit.Id
            });

            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
            Context.ChangeTracker.Clear();

            Context.StockItems.Add(new StockItem()
            {
                ProductId = testProduct.Id,
                ShelfId = firstTestShelf.Entity.Id,
                StockUnitId = testStockUnit.Id,
                Quantity = 50
            });

            Context.StockItems.Add(new StockItem()
            {
                ProductId = testProduct.Id,
                ShelfId = secondTestShelf.Entity.Id,
                StockUnitId = testStockUnit.Id,
                Quantity = 50
            });

            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
            Context.ChangeTracker.Clear();

            var stockItemService = CreateStockItemService();

            var result = await stockItemService.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, r => Assert.IsType<StockItem>(r));
        }

        #endregion

        #region Get

        [Fact]
        public async Task Get_ShouldThrowKeyNotFoundException_WhenStockItemDoesNotExist()
        {
            var nonExistentId = 999;

            var stockItemService = CreateStockItemService();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => stockItemService.Get(nonExistentId));
        }

        [Fact]
        public async Task Get_ShouldReturnStockItem_WhenStockItemExists()
        {
            var addedStockItem = await AddTestStockItem();

            var stockItemService = CreateStockItemService();

            var result = await stockItemService.Get(addedStockItem.Id);

            Assert.NotNull(result);
            Assert.Equal(addedStockItem.Id, result.Id);
            Assert.Equal(addedStockItem.ProductId, result.ProductId);
            Assert.Equal(addedStockItem.ShelfId, result.ShelfId);
            Assert.Equal(addedStockItem.StockUnitId, result.StockUnitId);
            Assert.Equal(addedStockItem.Quantity, result.Quantity);
        }

        #endregion

        #region Add

        [Fact]
        public async Task Add_ShouldThrowArgumentNullException_WhenStockItemIsNull()
        {
            var stockItemService = CreateStockItemService();

            await Assert.ThrowsAsync<ArgumentNullException>(() => stockItemService.Add(null!));
        }

        [Fact]
        public async Task Add_ShouldAddNewStockItemAndAssignId()
        {
            var newStockItem = new StockItem
            {
                ProductId = (await AddTestProduct()).Id,
                ShelfId = (await AddTestShelf()).Id,
                StockUnitId = (await AddTestStockUnit()).Id,
                Quantity = 50
            };

            var stockItemService = CreateStockItemService();

            var addedStockItem = await stockItemService.Add(newStockItem);

            Assert.NotNull(addedStockItem);

            // 1. Check if the ID was assigned by the database
            Assert.True(addedStockItem.Id > 0);
            Assert.Equal(newStockItem.ProductId, addedStockItem.ProductId);

            // 2. Verify it was actually saved to the database
            var retrievedStockItem = await Context.StockItems.FindAsync([addedStockItem.Id], TestContext.Current.CancellationToken);

            Assert.NotNull(retrievedStockItem);
            Assert.Equal(addedStockItem.Id, retrievedStockItem.Id);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Update_ShouldThrowKeyNotFoundException_WhenStockItemDoesNotExist()
        {
            var nonExistentStockItem = new StockItem
            {
                Id = 999,
                ProductId = (await AddTestProduct()).Id,
                ShelfId = (await AddTestShelf()).Id,
                StockUnitId = (await AddTestStockUnit()).Id,
                Quantity = 50
            };

            var stockItemService = CreateStockItemService();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => stockItemService.Update(nonExistentStockItem));
        }

        [Fact]
        public async Task Update_ShouldThrowArgumentNullException_WhenStockItemIsNull()
        {
            var stockItemService = CreateStockItemService();

            await Assert.ThrowsAsync<ArgumentNullException>(() => stockItemService.Update(null!));
        }

        [Fact]
        public async Task Update_ShouldUpdateExistingStockItem()
        {
            var stockItemToUpdate = await AddTestStockItem();

            var testProduct = Context.Products.Add(new Product()
            {
                Name = "Foo",
                Description = "Bar",
                ProductTypeId = (await Context.ProductTypes.FirstAsync(TestContext.Current.CancellationToken)).Id,
                StoreId = (await Context.Stores.FirstAsync(TestContext.Current.CancellationToken)).Id,
                PurchaseUnitId = (await Context.PurchaseUnits.FirstAsync(TestContext.Current.CancellationToken)).Id
            });

            var testShelf = Context.Shelves.Add(new Shelf()
            {
                Name = "Foo",
                StorageUnitId = (await Context.StorageUnits.FirstAsync(TestContext.Current.CancellationToken)).Id
            });

            var testStockUnit = Context.StockUnits.Add(new StockUnit()
            {
                Name = "Foo"
            });

            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
            Context.ChangeTracker.Clear();

            var newProductId = testProduct.Entity.Id;
            var newShelfId = testShelf.Entity.Id;
            var newStockUnitId = testStockUnit.Entity.Id;
            var newQuantity = stockItemToUpdate.Quantity + 100;

            stockItemToUpdate.ProductId = newProductId;
            stockItemToUpdate.ShelfId = newShelfId;
            stockItemToUpdate.StockUnitId = newStockUnitId;
            stockItemToUpdate.Quantity = newQuantity;

            var stockItemService = CreateStockItemService();

            var updatedStockItem = await stockItemService.Update(stockItemToUpdate);

            Assert.NotNull(updatedStockItem);
            Assert.Equal(stockItemToUpdate.Id, updatedStockItem.Id);
            Assert.Equal(stockItemToUpdate.ProductId, updatedStockItem.ProductId);
            Assert.Equal(stockItemToUpdate.ShelfId, updatedStockItem.ShelfId);
            Assert.Equal(stockItemToUpdate.StockUnitId, updatedStockItem.StockUnitId);
            Assert.Equal(stockItemToUpdate.Quantity, updatedStockItem.Quantity);

            // Verify the change was saved to the database
            var retrievedStockItem = await Context.StockItems.FindAsync([stockItemToUpdate.Id], TestContext.Current.CancellationToken);

            Assert.NotNull(retrievedStockItem);
            Assert.Equal(newProductId, retrievedStockItem.ProductId);
            Assert.Equal(newShelfId, retrievedStockItem.ShelfId);
            Assert.Equal(newStockUnitId, retrievedStockItem.StockUnitId);
            Assert.Equal(newQuantity, retrievedStockItem.Quantity);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete_ShouldDoNothing_WhenStockItemDoesNotExist()
        {
            var nonExistentId = 999;

            var stockItemService = CreateStockItemService();

            var ex = await Record.ExceptionAsync(() => stockItemService.Delete(nonExistentId));

            // Assert that no exception was thrown and the method simply returned
            Assert.Null(ex);
        }

        [Fact]
        public async Task Delete_ShouldRemoveStockItemFromDatabase()
        {
            var stockItemToDelete = await AddTestStockItem();

            //Delete created audits
            await Context.StockItemAudits.ExecuteDeleteAsync(TestContext.Current.CancellationToken);

            var stockItemService = CreateStockItemService();

            await stockItemService.Delete(stockItemToDelete.Id);

            var deletedStockItem = await Context.StockItems.FindAsync([stockItemToDelete.Id], TestContext.Current.CancellationToken);

            Assert.Null(deletedStockItem);
        }

        #endregion

        #region ValidateItem

        [Fact]
        public async Task ValidateItem_ShouldReturnEmpty_WhenProductIsUniqueOnShelf()
        {
            var product = await AddTestProduct();
            var shelf = await AddTestShelf();
            var stockUnit = await AddTestStockUnit();

            var stockItem = new StockItem
            {
                ProductId = product.Id,
                ShelfId = shelf.Id,
                StockUnitId = stockUnit.Id
            };

            var service = CreateStockItemService();

            var errors = await service.ValidateItem(stockItem);

            Assert.Empty(errors);
        }

        [Fact]
        public async Task ValidateItem_ShouldReturnError_WhenProductAlreadyExistsOnSameShelf()
        {
            var existingItem = await AddTestStockItem();

            var duplicateItem = new StockItem
            {
                ProductId = existingItem.ProductId,
                ShelfId = existingItem.ShelfId,
                StockUnitId = existingItem.StockUnitId
            };

            var service = CreateStockItemService();

            var errors = await service.ValidateItem(duplicateItem);

            var error = Assert.Single(errors);
            Assert.Equal(nameof(StockItem.ProductId), error.PropertyName);
            Assert.Equal("This product is already on this location..", error.ErrorMessage);
        }

        [Fact]
        public async Task ValidateItem_ShouldAllowSameProductOnDifferentShelves()
        {
            var product = await AddTestProduct();
            var shelfA = await AddTestShelf();
            var stockUnit = await AddTestStockUnit();

            var shelfB = Context.Shelves.Add(new Shelf()
            {
                Name = "Foo",
                StorageUnitId = shelfA.StorageUnitId
            });


            Context.StockItems.Add(new StockItem
            {
                ProductId = product.Id,
                ShelfId = shelfA.Id,
                StockUnitId = stockUnit.Id
            });

            await Context.SaveChangesAsync();
            Context.ChangeTracker.Clear();

            var itemForShelfB = new StockItem
            {
                ProductId = product.Id, 
                ShelfId = shelfB.Entity.Id,
                StockUnitId = stockUnit.Id
            };

            var service = CreateStockItemService();

            var errors = await service.ValidateItem(itemForShelfB);

            Assert.Empty(errors);
        }

        [Fact]
        public async Task ValidateItem_ShouldNotReturnError_WhenUpdatingExistingStockItem()
        {
            var existingItem = await AddTestStockItem();

            var service = CreateStockItemService();

            var errors = await service.ValidateItem(existingItem);

            Assert.Empty(errors);
        }

        #endregion
    }
}