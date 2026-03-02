using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.Services
{
	public class PurchaseUnitServiceIntegrationTests : BaseIntegrationTest
	{

		private PurchaseUnitService CreatePurchaseUnitService()
		{
			var contextFactory = new DbContextFactoryMock(Options, Context.Database.GetDbConnection());
			return new PurchaseUnitService(contextFactory);
		}

		#region All

		[Fact]
		public async Task All_ShouldReturnEmptyList_WhenNoPurchaseUnitsExist()
		{
			var purchaseUnitService = CreatePurchaseUnitService();

			var result = await purchaseUnitService.GetAll();

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task All_ShouldReturnAllPurchaseUnits()
		{
			await AddTestPurchaseUnit();

			//Add 2nd PurchaseUnit
			Context.PurchaseUnits.Add(new PurchaseUnit
			{
				Name = "Foo"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var purchaseUnitService = CreatePurchaseUnitService();

			var result = await purchaseUnitService.GetAll();

			Assert.NotNull(result);
			Assert.Equal(2, result.Count());
			Assert.All(result, r => Assert.IsType<PurchaseUnit>(r));
		}

		#endregion

		#region Get

		[Fact]
		public async Task Get_ShouldThrowKeyNotFoundException_WhenPurchaseUnitDoesNotExist()
		{
			var nonExistentId = 999;

			var purchaseUnitService = CreatePurchaseUnitService();

			await Assert.ThrowsAsync<KeyNotFoundException>(() => purchaseUnitService.Get(nonExistentId));
		}

		[Fact]
		public async Task Get_ShouldReturnPurchaseUnit_WhenPurchaseUnitExists()
		{
			var addedPurchaseUnit = await AddTestPurchaseUnit();

			var purchaseUnitService = CreatePurchaseUnitService();

			var result = await purchaseUnitService.Get(addedPurchaseUnit.Id);

			Assert.NotNull(result);
			Assert.Equal(addedPurchaseUnit.Id, result.Id);
			Assert.Equal(addedPurchaseUnit.Name, result.Name);
		}

		#endregion

		#region Add

		[Fact]
		public async Task Add_ShouldThrowArgumentNullException_WhenPurchaseUnitIsNull()
		{
			var purchaseUnitService = CreatePurchaseUnitService();

			await Assert.ThrowsAsync<ArgumentNullException>(() => purchaseUnitService.Add(null!));
		}

		[Fact]
		public async Task Add_ShouldAddNewPurchaseUnitAndAssignId()
		{
			var newPurchaseUnit = new PurchaseUnit
			{
				Name = "Foo"
			};

			var purchaseUnitService = CreatePurchaseUnitService();

			var addedPurchaseUnit = await purchaseUnitService.Add(newPurchaseUnit);

			Assert.NotNull(addedPurchaseUnit);

			// 1. Check if the ID was assigned by the database
			Assert.True(addedPurchaseUnit.Id > 0);
			Assert.Equal(newPurchaseUnit.Name, addedPurchaseUnit.Name);

			// 2. Verify it was actually saved to the database
			var retrievedPurchaseUnit = await Context.PurchaseUnits.FindAsync([addedPurchaseUnit.Id], TestContext.Current.CancellationToken);

			Assert.NotNull(retrievedPurchaseUnit);
			Assert.Equal(addedPurchaseUnit.Id, retrievedPurchaseUnit.Id);
		}

		#endregion

		#region Update

		[Fact]
		public async Task Update_ShouldThrowKeyNotFoundException_WhenPurchaseUnitDoesNotExist()
		{
			var nonExistentPurchaseUnit = new PurchaseUnit { Id = 999, Name = "Ghost PurchaseUnit" };

			var purchaseUnitService = CreatePurchaseUnitService();

			await Assert.ThrowsAsync<KeyNotFoundException>(() => purchaseUnitService.Update(nonExistentPurchaseUnit));
		}

		[Fact]
		public async Task Update_ShouldThrowArgumentNullException_WhenPurchaseUnitIsNull()
		{
			var purchaseUnitService = CreatePurchaseUnitService();

			await Assert.ThrowsAsync<ArgumentNullException>(() => purchaseUnitService.Update(null!));
		}

		[Fact]
		public async Task Update_ShouldUpdateExistingPurchaseUnit()
		{
			var newName = "Foo";

			var purchaseUnitToUpdate = await AddTestPurchaseUnit();

			purchaseUnitToUpdate.Name = newName;

			var purchaseUnitService = CreatePurchaseUnitService();

			var updatedPurchaseUnit = await purchaseUnitService.Update(purchaseUnitToUpdate);

			Assert.NotNull(updatedPurchaseUnit);
			Assert.Equal(purchaseUnitToUpdate.Id, updatedPurchaseUnit.Id);
			Assert.Equal(newName, updatedPurchaseUnit.Name);

			// Verify the change was saved to the database
			var retrievedPurchaseUnit = await Context.PurchaseUnits.FindAsync([purchaseUnitToUpdate.Id], TestContext.Current.CancellationToken);

			Assert.NotNull(retrievedPurchaseUnit);
			Assert.Equal(newName, retrievedPurchaseUnit.Name);
		}

		#endregion

		#region Delete

		[Fact]
		public async Task Delete_ShouldDoNothing_WhenPurchaseUnitDoesNotExist()
		{
			var nonExistentId = 999;

			var purchaseUnitService = CreatePurchaseUnitService();

			var ex = await Record.ExceptionAsync(() => purchaseUnitService.Delete(nonExistentId));

			// Assert that no exception was thrown and the method simply returned
			Assert.Null(ex);
		}

		[Fact]
		public async Task Delete_ShouldRemovePurchaseUnitFromDatabase()
		{
			var purchaseUnitToDelete = await AddTestPurchaseUnit();

			var purchaseUnitService = CreatePurchaseUnitService();

			await purchaseUnitService.Delete(purchaseUnitToDelete.Id);

			var deletedPurchaseUnit = await Context.PurchaseUnits.FindAsync([purchaseUnitToDelete.Id], TestContext.Current.CancellationToken);

			Assert.Null(deletedPurchaseUnit);
		}

		#endregion

		#region ValidateItem

		[Fact]
		public async Task ValidateItem_ShouldReturnEmpty_WhenPurchaseUnitNameIsUnique()
		{
			var purchaseUnit = new PurchaseUnit { Name = "Foo" };

			var service = CreatePurchaseUnitService();

			var errors = await service.ValidateItem(purchaseUnit);

			Assert.Empty(errors);
		}

		[Fact]
		public async Task ValidateItem_ShouldReturnError_WhenPurchaseUnitNameAlreadyExists()
		{
			var existingPurchaseUnit = await AddTestPurchaseUnit();

			var newPurchaseUnit = new PurchaseUnit
			{
				Name = existingPurchaseUnit.Name
			};

			var service = CreatePurchaseUnitService();

			var errors = await service.ValidateItem(newPurchaseUnit);

			var error = Assert.Single(errors);
			Assert.Equal(nameof(ProductType.Name), error.PropertyName);
			Assert.Equal("A purchase unit with this name already exists.", error.ErrorMessage);
		}

		[Fact]
		public async Task ValidateItem_ShouldNotReturnError_WhenUpdatingExistingPurchaseUnitWithSameName()
		{
			var existingPurchaseUnite = await AddTestPurchaseUnit();

			var service = CreatePurchaseUnitService();

			var errors = await service.ValidateItem(existingPurchaseUnite);

			Assert.Empty(errors);
		}

		#endregion

	}
}