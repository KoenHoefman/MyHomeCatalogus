using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.Services
{
    public class ShelfServiceIntegrationTests : BaseIntegrationTest
    {

        private ShelfService CreateShelfService()
        {
            var contextFactory = new DbContextFactoryMock(Options, Context.Database.GetDbConnection());
            return new ShelfService(contextFactory);
        }

        #region GetAll

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoShelfsExist()
        {
            var shelfService = CreateShelfService();

            var result = await shelfService.GetAll();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllShelfs()
        {
            var testShelf = await AddTestShelf();

            //Add 2nd Shelf
            Context.Shelves.Add(new Shelf
            {
                Name = "Foo",
                Description = "Bar",
                StorageUnitId = testShelf.StorageUnitId
            });

            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
            Context.ChangeTracker.Clear();

            var shelfService = CreateShelfService();

            var result = await shelfService.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, r => Assert.IsType<Shelf>(r));
        }

        #endregion

        #region Get

        [Fact]
        public async Task Get_ShouldThrowKeyNotFoundException_WhenShelfDoesNotExist()
        {
            var nonExistentId = 999;

            var shelfService = CreateShelfService();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => shelfService.Get(nonExistentId));
        }

        [Fact]
        public async Task Get_ShouldReturnShelf_WhenShelfExists()
        {
            var addedShelf = await AddTestShelf();

            var shelfService = CreateShelfService();

            var result = await shelfService.Get(addedShelf.Id);

            Assert.NotNull(result);
            Assert.Equal(addedShelf.Id, result.Id);
            Assert.Equal(addedShelf.Name, result.Name);
            Assert.Equal(addedShelf.Description, result.Description);
        }

        #endregion

        #region Add

        [Fact]
        public async Task Add_ShouldThrowArgumentNullException_WhenShelfIsNull()
        {
            var shelfService = CreateShelfService();

            await Assert.ThrowsAsync<ArgumentNullException>(() => shelfService.Add(null!));
        }

        [Fact]
        public async Task Add_ShouldAddNewShelfAndAssignId()
        {
            var testStorageUnit = await AddTestStorageUnit();

            var newShelf = new Shelf
            {
                Name = "Foo",
                Description = "Bar",
                StorageUnitId = testStorageUnit.Id
            };

            var shelfService = CreateShelfService();

            var addedShelf = await shelfService.Add(newShelf);

            Assert.NotNull(addedShelf);

            Assert.True(addedShelf.Id > 0);
            Assert.Equal(newShelf.Name, addedShelf.Name);

            var retrievedShelf = await Context.Shelves.FindAsync([addedShelf.Id], TestContext.Current.CancellationToken);

            Assert.NotNull(retrievedShelf);
            Assert.Equal(addedShelf.Id, retrievedShelf.Id);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Update_ShouldThrowKeyNotFoundException_WhenShelfDoesNotExist()
        {
            var nonExistentShelf = new Shelf { Id = 999, Name = "Ghost Shelf" };

            var shelfService = CreateShelfService();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => shelfService.Update(nonExistentShelf));
        }

        [Fact]
        public async Task Update_ShouldThrowArgumentNullException_WhenShelfIsNull()
        {
            var shelfService = CreateShelfService();

            await Assert.ThrowsAsync<ArgumentNullException>(() => shelfService.Update(null!));
        }

        [Fact]
        public async Task Update_ShouldUpdateExistingShelf()
        {
            var newName = "Foo";
            var newDescription = "Bar";

            var shelfToUpdate = await AddTestShelf();

            shelfToUpdate.Name = newName;
            shelfToUpdate.Description = newDescription;

            var shelfService = CreateShelfService();

            var updatedShelf = await shelfService.Update(shelfToUpdate);

            Assert.NotNull(updatedShelf);
            Assert.Equal(shelfToUpdate.Id, updatedShelf.Id);
            Assert.Equal(newName, updatedShelf.Name);
            Assert.Equal(newDescription, updatedShelf.Description);

            var retrievedShelf = await Context.Shelves.FindAsync([shelfToUpdate.Id], TestContext.Current.CancellationToken);

            Assert.NotNull(retrievedShelf);
            Assert.Equal(newName, retrievedShelf.Name);
            Assert.Equal(newDescription, retrievedShelf.Description);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete_ShouldDoNothing_WhenShelfDoesNotExist()
        {
            var nonExistentId = 999;

            var shelfService = CreateShelfService();

            var ex = await Record.ExceptionAsync(() => shelfService.Delete(nonExistentId));

            Assert.Null(ex);
        }

        [Fact]
        public async Task Delete_ShouldRemoveShelfFromDatabase()
        {
            var shelfToDelete = await AddTestShelf();

            var shelfService = CreateShelfService();

            await shelfService.Delete(shelfToDelete.Id);

            var deletedShelf = await Context.Shelves.FindAsync([shelfToDelete.Id], TestContext.Current.CancellationToken);

            Assert.Null(deletedShelf);
        }

        #endregion

        #region ValidateItem

        [Fact]
        public async Task ValidateItem_ShouldReturnEmpty_WhenShelfNameIsUniqueInStorageUnit()
        {
            var storageUnit = await AddTestStorageUnit();

            var shelf = new Shelf
            {
                Name = "Foo", 
                StorageUnitId = storageUnit.Id
            };

            var service = CreateShelfService();

            var errors = await service.ValidateItem(shelf);

            Assert.Empty(errors);
        }

        [Fact]
        public async Task ValidateItem_ShouldReturnError_WhenShelfNameAlreadyExistsInSameStorageUnit()
        {
            var existingShelf = await AddTestShelf();

            var newShelf = new Shelf
            {
                Name = existingShelf.Name,
                StorageUnitId = existingShelf.StorageUnitId
            };
            var service = CreateShelfService();

            var errors = await service.ValidateItem(newShelf);

            var error = Assert.Single(errors);
            Assert.Equal(nameof(Shelf.Name), error.PropertyName);
            Assert.Equal("A shelf with this name already exists in this storage unit.", error.ErrorMessage);
        }

        [Fact]
        public async Task ValidateItem_ShouldNotReturnError_WhenSameShelfNameExistsInDifferentStorageUnit()
        {
            var unitA = await AddTestStorageUnit();
            var unitB = Context.StorageUnits.Add(new StorageUnit()
            {
                Name = "Foo",
                RoomId = unitA.RoomId
            });

            var shelfA = new Shelf { 
                Name = "Bar", 
                StorageUnitId = unitA.Id
            };

            await Context.SaveChangesAsync();
            Context.ChangeTracker.Clear();

            var shelfB = new Shelf
            {
                Name = shelfA.Name,
                StorageUnitId = unitB.Entity.Id
            };

            var service = CreateShelfService();

            var errors = await service.ValidateItem(shelfB);

            Assert.Empty(errors);
        }

        [Fact]
        public async Task ValidateItem_ShouldNotReturnError_WhenUpdatingExistingShelfWithSameName()
        {
            var existingShelf = await AddTestShelf();

            var service = CreateShelfService();

            var errors = await service.ValidateItem(existingShelf);

            Assert.Empty(errors);
        }

        #endregion
    }
}
