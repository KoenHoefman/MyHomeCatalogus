using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.Services
{
	public class ProductTypeServiceIntegrationTests : BaseIntegrationTest
	{

		private ProductTypeService CreateProductTypeService()
		{
			var contextFactory = new DbContextFactoryMock(Options, Context.Database.GetDbConnection());
			return new ProductTypeService(contextFactory, NullLogger<ProductTypeService>.Instance);
		}

		#region GetAll

		[Fact]
		public async Task GetAll_ShouldReturnEmptyList_WhenNoProductTypesExist()
		{
			var productTypeService = CreateProductTypeService();

			var result = await productTypeService.GetAll();

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAll_ShouldReturnAll()
		{
			await AddTestProductType();

			//Add 2nd ProductType
			Context.ProductTypes.Add(new ProductType
			{
				Name = "Foo",
				Description = "Bar"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var productTypeService = CreateProductTypeService();

			var result = await productTypeService.GetAll();

			Assert.NotNull(result);
			Assert.Equal(2, result.Count());
			Assert.All(result, r => Assert.IsType<ProductType>(r));
		}

		#endregion

		#region Get

		[Fact]
		public async Task Get_ShouldThrowKeyNotFoundException_WhenProductTypeDoesNotExist()
		{
			var nonExistentId = 999;

			var productTypeService = CreateProductTypeService();

			await Assert.ThrowsAsync<KeyNotFoundException>(() => productTypeService.Get(nonExistentId));
		}

		[Fact]
		public async Task Get_ShouldReturnProductType_WhenProductTypeExists()
		{
			var addedProductType = await AddTestProductType();

			var productTypeService = CreateProductTypeService();

			var result = await productTypeService.Get(addedProductType.Id);

			Assert.NotNull(result);
			Assert.Equal(addedProductType.Id, result.Id);
			Assert.Equal(addedProductType.Name, result.Name);
			Assert.Equal(addedProductType.Description, result.Description);
		}

		#endregion

		#region Add

		[Fact]
		public async Task Add_ShouldThrowArgumentNullException_WhenProductTypeIsNull()
		{
			var productTypeService = CreateProductTypeService();

			await Assert.ThrowsAsync<ArgumentNullException>(() => productTypeService.Add(null!));
		}

		[Fact]
		public async Task Add_ShouldAddNewProductTypeAndAssignId()
		{
			var newProductType = new ProductType
			{
				Name = "Foo",
				Description = "Bar"
			};

			var productTypeService = CreateProductTypeService();

			var addedProductType = await productTypeService.Add(newProductType);

			Assert.NotNull(addedProductType);

			// 1. Check if the ID was assigned by the database
			Assert.True(addedProductType.Id > 0);
			Assert.Equal(newProductType.Name, addedProductType.Name);

			// 2. Verify it was actually saved to the database
			var retrievedProductType = await Context.ProductTypes.FindAsync([addedProductType.Id], TestContext.Current.CancellationToken);

			Assert.NotNull(retrievedProductType);
			Assert.Equal(addedProductType.Id, retrievedProductType.Id);
		}

		#endregion

		#region Update

		[Fact]
		public async Task Update_ShouldThrowKeyNotFoundException_WhenProductTypeDoesNotExist()
		{
			var nonExistentProductType = new ProductType { Id = 999, Name = "Ghost ProductType" };

			var productTypeService = CreateProductTypeService();

			await Assert.ThrowsAsync<KeyNotFoundException>(() => productTypeService.Update(nonExistentProductType));
		}

		[Fact]
		public async Task Update_ShouldThrowArgumentNullException_WhenProductTypeIsNull()
		{
			var productTypeService = CreateProductTypeService();

			await Assert.ThrowsAsync<ArgumentNullException>(() => productTypeService.Update(null!));
		}

		[Fact]
		public async Task Update_ShouldUpdateExistingProductType()
		{
			var newName = "Foo";
			var newDescription = "Bar";

			var productTypeToUpdate = await AddTestProductType();

			productTypeToUpdate.Name = newName;
			productTypeToUpdate.Description = newDescription;

			var productTypeService = CreateProductTypeService();

			var updatedProductType = await productTypeService.Update(productTypeToUpdate);

			Assert.NotNull(updatedProductType);
			Assert.Equal(productTypeToUpdate.Id, updatedProductType.Id);
			Assert.Equal(newName, updatedProductType.Name);
			Assert.Equal(newDescription, updatedProductType.Description);

			// Verify the change was saved to the database
			var retrievedProductType = await Context.ProductTypes.FindAsync([productTypeToUpdate.Id], TestContext.Current.CancellationToken);

			Assert.NotNull(retrievedProductType);
			Assert.Equal(newName, retrievedProductType.Name);
			Assert.Equal(newDescription, retrievedProductType.Description);
		}

		#endregion

		#region Delete

		[Fact]
		public async Task Delete_ShouldDoNothing_WhenProductTypeDoesNotExist()
		{
			var nonExistentId = 999;

			var productTypeService = CreateProductTypeService();

			var ex = await Record.ExceptionAsync(() => productTypeService.Delete(nonExistentId));

			// Assert that no exception was thrown and the method simply returned
			Assert.Null(ex);
		}

		[Fact]
		public async Task Delete_ShouldRemoveProductTypeFromDatabase()
		{
			var productTypeToDelete = await AddTestProductType();

			var productTypeService = CreateProductTypeService();

			await productTypeService.Delete(productTypeToDelete.Id);

			var deletedProductType = await Context.ProductTypes.FindAsync([productTypeToDelete.Id], TestContext.Current.CancellationToken);

			Assert.Null(deletedProductType);
		}

		#endregion

		#region ValidateItem

		[Fact]
		public async Task ValidateItem_ShouldReturnEmpty_WhenProductTypeNameIsUnique()
		{
			var productType = new ProductType { Name = "Foo" };

			var service = CreateProductTypeService();

			var errors = await service.ValidateItem(productType);

			Assert.Empty(errors);
		}

		[Fact]
		public async Task ValidateItem_ShouldReturnError_WhenProductTypeNameAlreadyExists()
		{
			var existingProductType = await AddTestProductType();

			var newProductType = new ProductType
			{
				Name = existingProductType.Name
			};

			var service = CreateProductTypeService();

			var errors = await service.ValidateItem(newProductType);

			var error = Assert.Single(errors);
			Assert.Equal(nameof(ProductType.Name), error.PropertyName);
			Assert.Equal("A product type with this name already exists.", error.ErrorMessage);
		}

		[Fact]
		public async Task ValidateItem_ShouldNotReturnError_WhenUpdatingExistingProductTypeWithSameName()
		{
			var existingProductType = await AddTestProductType();

			var service = CreateProductTypeService();

			var errors = await service.ValidateItem(existingProductType);

			Assert.Empty(errors);
		}

		#endregion
	}
}