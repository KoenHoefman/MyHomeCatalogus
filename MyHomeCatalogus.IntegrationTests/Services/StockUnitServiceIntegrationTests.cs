using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.Services
{
    public class StockUnitServiceIntegrationTests : BaseIntegrationTest
    {

        private StockUnitService CreateStockUnitService()
        {
            var contextFactory = new DbContextFactoryMock(Options, Context.Database.GetDbConnection());
            return new StockUnitService(contextFactory);
        }

        #region GetAll

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoStockUnitsExist()
        {
            var stockUnitService = CreateStockUnitService();

            var result = await stockUnitService.GetAll();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllStockUnits()
        {
            await AddTestStockUnit();

            //Add 2nd StockUnit
            Context.StockUnits.Add(new StockUnit
            {
                Name = "Foo"
            });

            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
            Context.ChangeTracker.Clear();

            var stockUnitService = CreateStockUnitService();

            var result = await stockUnitService.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, r => Assert.IsType<StockUnit>(r));
        }

        #endregion

        #region Get

        [Fact]
        public async Task Get_ShouldThrowKeyNotFoundException_WhenStockUnitDoesNotExist()
        {
            var nonExistentId = 999;

            var stockUnitService = CreateStockUnitService();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => stockUnitService.Get(nonExistentId));
        }

        [Fact]
        public async Task Get_ShouldReturnStockUnit_WhenStockUnitExists()
        {
            var addedStockUnit = await AddTestStockUnit();

            var stockUnitService = CreateStockUnitService();

            var result = await stockUnitService.Get(addedStockUnit.Id);

            Assert.NotNull(result);
            Assert.Equal(addedStockUnit.Id, result.Id);
            Assert.Equal(addedStockUnit.Name, result.Name);
        }

        #endregion

        #region Add

        [Fact]
        public async Task Add_ShouldThrowArgumentNullException_WhenStockUnitIsNull()
        {
            var stockUnitService = CreateStockUnitService();

            await Assert.ThrowsAsync<ArgumentNullException>(() => stockUnitService.Add(null!));
        }

        [Fact]
        public async Task Add_ShouldAddNewStockUnitAndAssignId()
        {
            var newStockUnit = new StockUnit
            {
                Name = "Foo"
            };

            var stockUnitService = CreateStockUnitService();

            var addedStockUnit = await stockUnitService.Add(newStockUnit);

            Assert.NotNull(addedStockUnit);

            Assert.True(addedStockUnit.Id > 0);
            Assert.Equal(newStockUnit.Name, addedStockUnit.Name);

            var retrievedStockUnit = await Context.StockUnits.FindAsync([addedStockUnit.Id], TestContext.Current.CancellationToken);

            Assert.NotNull(retrievedStockUnit);
            Assert.Equal(addedStockUnit.Id, retrievedStockUnit.Id);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Update_ShouldThrowKeyNotFoundException_WhenStockUnitDoesNotExist()
        {
            var nonExistentStockUnit = new StockUnit { Id = 999, Name = "Ghost StockUnit" };

            var stockUnitService = CreateStockUnitService();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => stockUnitService.Update(nonExistentStockUnit));
        }

        [Fact]
        public async Task Update_ShouldThrowArgumentNullException_WhenStockUnitIsNull()
        {
            var stockUnitService = CreateStockUnitService();

            await Assert.ThrowsAsync<ArgumentNullException>(() => stockUnitService.Update(null!));
        }

        [Fact]
        public async Task Update_ShouldUpdateExistingStockUnit()
        {
            var newName = "Foo";

            var stockUnitToUpdate = await AddTestStockUnit();

            stockUnitToUpdate.Name = newName;

            var stockUnitService = CreateStockUnitService();

            var updatedStockUnit = await stockUnitService.Update(stockUnitToUpdate);

            Assert.NotNull(updatedStockUnit);
            Assert.Equal(stockUnitToUpdate.Id, updatedStockUnit.Id);
            Assert.Equal(newName, updatedStockUnit.Name);

            var retrievedStockUnit = await Context.StockUnits.FindAsync([stockUnitToUpdate.Id], TestContext.Current.CancellationToken);

            Assert.NotNull(retrievedStockUnit);
            Assert.Equal(newName, retrievedStockUnit.Name);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete_ShouldDoNothing_WhenStockUnitDoesNotExist()
        {
            var nonExistentId = 999;

            var stockUnitService = CreateStockUnitService();

            var ex = await Record.ExceptionAsync(() => stockUnitService.Delete(nonExistentId));

            // Assert that no exception was thrown and the method simply returned
            Assert.Null(ex);
        }

        [Fact]
        public async Task Delete_ShouldRemoveStockUnitFromDatabase()
        {
            var stockUnitToDelete = await AddTestStockUnit();

            var stockUnitService = CreateStockUnitService();

            await stockUnitService.Delete(stockUnitToDelete.Id);

            var deletedStockUnit = await Context.StockUnits.FindAsync([stockUnitToDelete.Id], TestContext.Current.CancellationToken);

            Assert.Null(deletedStockUnit);
        }

        #endregion

        #region ValidateItem

        [Fact]
        public async Task ValidateItem_ShouldReturnEmpty_WhenStockUnitNameIsUnique()
        {
            var stockUnit = new StockUnit { Name = "Foo" };

            var service = CreateStockUnitService();

            var errors = await service.ValidateItem(stockUnit);

            Assert.Empty(errors);
        }

        [Fact]
        public async Task ValidateItem_ShouldReturnError_WhenStockUnitNameAlreadyExists()
        {
            var existingStockUnit = await AddTestStockUnit();

            var newStockUnit = new StockUnit
            {
                Name = existingStockUnit.Name
            };

            var service = CreateStockUnitService();

            var errors = await service.ValidateItem(newStockUnit);

            var error = Assert.Single(errors);
            Assert.Equal(nameof(ProductType.Name), error.PropertyName);
            Assert.Equal("A stock unit with this name already exists.", error.ErrorMessage);
        }

        [Fact]
        public async Task ValidateItem_ShouldNotReturnError_WhenUpdatingExistingStockUnitWithSameName()
        {
            var existingStockUnite = await AddTestStockUnit();

            var service = CreateStockUnitService();

            var errors = await service.ValidateItem(existingStockUnite);

            Assert.Empty(errors);
        }

        #endregion

    }
}