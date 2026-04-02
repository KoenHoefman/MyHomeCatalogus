using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.Services
{
	public class ProductServiceIntegrationTests : BaseIntegrationTest
	{

		private ProductService CreateProductService()
		{
			var contextFactory = new DbContextFactoryMock(Options, Context.Database.GetDbConnection());
			return new ProductService(contextFactory, NullLogger<ProductService>.Instance);
		}

		#region GetAll

		[Fact]
		public async Task GetAllProducts_ShouldReturnEmptyList_WhenNoProductsExist()
		{
			var productService = CreateProductService();

			var result = await productService.GetAll();

			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAllProducts_ShouldReturnAllProducts()
		{
			var firstProduct = await AddTestProduct();

			//Add 2nd Product
			Context.Products.Add(new Product
			{
				Name = "Foo",
				Description = "Bar",
				ProductTypeId = firstProduct.ProductTypeId,
				PurchaseUnitId = firstProduct.PurchaseUnitId,
				StoreId = firstProduct.StoreId,
				Picture = firstProduct.Picture,
				PictureMimeType = firstProduct.PictureMimeType,
				Barcode = firstProduct.Barcode,
				BarcodeMimeType = firstProduct.BarcodeMimeType
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var productService = CreateProductService();

			var result = await productService.GetAll();

			Assert.NotNull(result);
			Assert.Equal(2, result.Count());
			Assert.All(result, r => Assert.IsType<Product>(r));
		}

		[Fact]
		public async Task GetAll_ShouldReturnFilteredProducts_WhenFilterIsApplied()
		{
			var type = await AddTestProductType();
			var unit = await AddTestPurchaseUnit();
			var store = await AddTestStore();

			Context.Products.AddRange(
			   new Product { Name = "Apple", ProductTypeId = type.Id, PurchaseUnitId = unit.Id, StoreId = store.Id },
			   new Product { Name = "Banana", ProductTypeId = type.Id, PurchaseUnitId = unit.Id, StoreId = store.Id },
			   new Product { Name = "Apricot", ProductTypeId = type.Id, PurchaseUnitId = unit.Id, StoreId = store.Id }
		   );

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var productService = CreateProductService();

			var result = await productService.GetAll(p => p.Name.StartsWith("A"));

			Assert.NotNull(result);
			Assert.Equal(2, result.Count());
			Assert.Contains(result, p => p.Name == "Apple");
			Assert.Contains(result, p => p.Name == "Apricot");
			Assert.DoesNotContain(result, p => p.Name == "Banana");
		}

		[Fact]
		public async Task GetAll_ShouldReturnEmpty_WhenFilterMatchesNothing()
		{
			await AddTestProduct();

			var productService = CreateProductService();

			var result = await productService.GetAll(p => p.Name == "NonExistentProductName123");

			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAll_ShouldReturnFilteredProducts_WhenFilteringByForeignId()
		{
			var typeA = await AddTestProductType();
			var purchaseUnit = await AddTestPurchaseUnit();
			var store = await AddTestStore();

			var typeB = Context.ProductTypes.Add(new ProductType
			{
				Name = "Foo",
				Description = "Bar"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			await Context.Products.AddRangeAsync(
				new Product { Name = "Product 1", ProductTypeId = typeA.Id, PurchaseUnitId = purchaseUnit.Id, StoreId = store.Id },
				new Product { Name = "Product 2", ProductTypeId = typeB.Entity.Id, PurchaseUnitId = purchaseUnit.Id, StoreId = store.Id }
			);

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var productService = CreateProductService();

			var result = await productService.GetAll(p => p.ProductTypeId == typeA.Id);

			Assert.Single(result);
			Assert.Equal("Product 1", result.Single().Name);
		}
		#endregion

		#region Get

		[Fact]
		public async Task Get_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
		{
			var nonExistentId = 999;

			var productService = CreateProductService();

			await Assert.ThrowsAsync<KeyNotFoundException>(() => productService.Get(nonExistentId));
		}

		[Fact]
		public async Task Get_ShouldReturnProduct_WhenProductExists()
		{
			var addedProduct = await AddTestProduct();

			var productService = CreateProductService();

			var result = await productService.Get(addedProduct.Id);

			Assert.NotNull(result);
			Assert.Equal(addedProduct.Id, result.Id);
			Assert.Equal(addedProduct.Name, result.Name);
			Assert.Equal(addedProduct.Description, result.Description);
			Assert.Equal(addedProduct.ProductTypeId, result.ProductTypeId);
			Assert.Equal(addedProduct.PurchaseUnitId, result.PurchaseUnitId);
			Assert.Equal(addedProduct.StoreId, result.StoreId);
			Assert.Equal(addedProduct.Picture, result.Picture);
			Assert.Equal(addedProduct.PictureMimeType, result.PictureMimeType);
			Assert.Equal(addedProduct.Barcode, result.Barcode);
			Assert.Equal(addedProduct.BarcodeMimeType, result.BarcodeMimeType);
		}

		#endregion

		#region Add

		[Fact]
		public async Task Add_ShouldThrowArgumentNullException_WhenProductIsNull()
		{
			var productService = CreateProductService();

			await Assert.ThrowsAsync<ArgumentNullException>(() => productService.Add(null!));
		}

		[Fact]
		public async Task Add_ShouldAddNewProductAndAssignId()
		{
			var newProduct = new Product
			{
				Name = "Foo",
				Description = "Bar",
				ProductTypeId = (await AddTestProductType()).Id,
				PurchaseUnitId = (await AddTestPurchaseUnit()).Id,
				StoreId = (await AddTestStore()).Id,
				Picture = [0x01, 0x02, 0x03, 0x04],
				PictureMimeType = "image/jpeg",
				Barcode = [0x05, 0x06, 0x07, 0x08],
				BarcodeMimeType = "image/jpeg"
			};

			var productService = CreateProductService();

			var addedProduct = await productService.Add(newProduct);

			Assert.NotNull(addedProduct);

			Assert.True(addedProduct.Id > 0);
			Assert.Equal(newProduct.Name, addedProduct.Name);

			var retrievedProduct = await Context.Products.FindAsync([addedProduct.Id], TestContext.Current.CancellationToken);

			Assert.NotNull(retrievedProduct);
			Assert.Equal(addedProduct.Id, retrievedProduct.Id);
			Assert.Equal(addedProduct.Name, retrievedProduct.Name);
			Assert.Equal(addedProduct.Description, retrievedProduct.Description);
			Assert.Equal(addedProduct.ProductTypeId, retrievedProduct.ProductTypeId);
			Assert.Equal(addedProduct.PurchaseUnitId, retrievedProduct.PurchaseUnitId);
			Assert.Equal(addedProduct.StoreId, retrievedProduct.StoreId);
			Assert.Equal(addedProduct.Picture, retrievedProduct.Picture);
			Assert.Equal(addedProduct.PictureMimeType, retrievedProduct.PictureMimeType);
			Assert.Equal(addedProduct.Barcode, retrievedProduct.Barcode);
			Assert.Equal(addedProduct.BarcodeMimeType, retrievedProduct.BarcodeMimeType);
		}

		#endregion

		#region Update

		[Fact]
		public async Task Update_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
		{
			var nonExistentProduct = new Product
			{
				Id = 999,
				Name = "Ghost Product",
				ProductTypeId = (await AddTestProductType()).Id,
				PurchaseUnitId = (await AddTestPurchaseUnit()).Id,
				StoreId = (await AddTestStore()).Id
			};

			var productService = CreateProductService();

			await Assert.ThrowsAsync<KeyNotFoundException>(() => productService.Update(nonExistentProduct));
		}

		[Fact]
		public async Task Update_ShouldThrowArgumentNullException_WhenProductIsNull()
		{
			var productService = CreateProductService();

			await Assert.ThrowsAsync<ArgumentNullException>(() => productService.Update(null!));
		}

		[Fact]
		public async Task Update_ShouldUpdateExistingProduct()
		{
			var newName = "Foo";
			var newDescription = "Bar";
			var newStore = Context.Stores.Add(new Store()
			{
				Name = "Foo"
			});
			var newProductType = Context.ProductTypes.Add(new ProductType()
			{
				Name = "Foo"
			});
			var newPurchaseUnit = Context.PurchaseUnits.Add(new PurchaseUnit()
			{
				Name = "Foo"
			});
			byte[] newPicture = [0x05, 0x06, 0x07, 0x08];
			var newPictureMimeType = "image/png";
			byte[] newBarcode = [0x05, 0x06, 0x07, 0x08];
			var newBarcodeMimeType = "image/png";

			var productToUpdate = await AddTestProduct();

			productToUpdate.Name = newName;
			productToUpdate.Description = newDescription;
			productToUpdate.ProductTypeId = newProductType.Entity.Id;
			productToUpdate.PurchaseUnitId = newPurchaseUnit.Entity.Id;
			productToUpdate.StoreId = newStore.Entity.Id;
			productToUpdate.Picture = newPicture;
			productToUpdate.PictureMimeType = newPictureMimeType;
			productToUpdate.Barcode = newBarcode;
			productToUpdate.BarcodeMimeType = newBarcodeMimeType;

			var productService = CreateProductService();

			var updatedProduct = await productService.Update(productToUpdate);

			Assert.NotNull(updatedProduct);
			Assert.Equal(productToUpdate.Id, updatedProduct.Id);
			Assert.Equal(newName, updatedProduct.Name);
			Assert.Equal(newDescription, updatedProduct.Description);
			Assert.Equal(newProductType.Entity.Id, updatedProduct.ProductTypeId);
			Assert.Equal(newPurchaseUnit.Entity.Id, updatedProduct.PurchaseUnitId);
			Assert.Equal(newStore.Entity.Id, updatedProduct.StoreId);
			Assert.Equal(newPicture, updatedProduct.Picture);
			Assert.Equal(newPictureMimeType, updatedProduct.PictureMimeType);
			Assert.Equal(newBarcode, updatedProduct.Barcode);
			Assert.Equal(newBarcodeMimeType, updatedProduct.BarcodeMimeType);

			var retrievedProduct = await Context.Products.FindAsync([productToUpdate.Id], TestContext.Current.CancellationToken);

			Assert.NotNull(retrievedProduct);
			Assert.Equal(newName, retrievedProduct.Name);
			Assert.Equal(newDescription, retrievedProduct.Description);
			Assert.Equal(newProductType.Entity.Id, retrievedProduct.ProductTypeId);
			Assert.Equal(newPurchaseUnit.Entity.Id, retrievedProduct.PurchaseUnitId);
			Assert.Equal(newStore.Entity.Id, retrievedProduct.StoreId);
			Assert.Equal(newPicture, retrievedProduct.Picture);
			Assert.Equal(newPictureMimeType, retrievedProduct.PictureMimeType);
			Assert.Equal(newBarcode, retrievedProduct.Barcode);
			Assert.Equal(newBarcodeMimeType, retrievedProduct.BarcodeMimeType);
		}

		#endregion

		#region Delete

		[Fact]
		public async Task Delete_ShouldDoNothing_WhenProductDoesNotExist()
		{
			var nonExistentId = 999;

			var productService = CreateProductService();

			var ex = await Record.ExceptionAsync(() => productService.Delete(nonExistentId));

			// Assert that no exception was thrown and the method simply returned
			Assert.Null(ex);
		}

		[Fact]
		public async Task Delete_ShouldRemoveProductFromDatabase()
		{
			var productToDelete = await AddTestProduct();

			var productService = CreateProductService();

			await productService.Delete(productToDelete.Id);

			var deletedProduct = await Context.Products.FindAsync([productToDelete.Id], TestContext.Current.CancellationToken);

			Assert.Null(deletedProduct);
		}

		#endregion

		#region ValidateItem

		[Fact]
		public async Task ValidateItem_ShouldReturnEmpty_WhenProductIsUnique()
		{
			var product = new Product
			{
				Name = "Foo",
				ProductTypeId = (await AddTestProductType()).Id,
				StoreId = (await AddTestStore()).Id,
				PurchaseUnitId = (await AddTestPurchaseUnit()).Id,
				Picture = [0x01, 0x02, 0x03, 0x04],
				PictureMimeType = "image/jpeg",
				Barcode = [0x05, 0x06, 0x07, 0x08],
				BarcodeMimeType = "image/jpeg"
			};

			var productService = CreateProductService();

			var errors = await productService.ValidateItem(product);

			Assert.Empty(errors);
		}

		[Fact]
		public async Task ValidateItem_ShouldReturnError_WhenProductNameExistsInSameStore()
		{
			var existingProduct = await AddTestProduct();

			var newProduct = new Product
			{
				Name = existingProduct.Name,
				ProductTypeId = existingProduct.ProductTypeId,
				StoreId = existingProduct.StoreId,
				PurchaseUnitId = existingProduct.PurchaseUnitId,
				Picture = [0x01, 0x02, 0x03, 0x04],
				PictureMimeType = "image/jpeg",
				Barcode = [0x05, 0x06, 0x07, 0x08],
				BarcodeMimeType = "image/jpeg"
			};

			var productService = CreateProductService();

			var errors = await productService.ValidateItem(newProduct);

			var error = Assert.Single(errors);
			Assert.Equal(nameof(Product.Name), error.PropertyName);
			Assert.Equal("A product with this name already exists for this store.", error.ErrorMessage);
		}

		[Fact]
		public async Task ValidateItem_ShouldNotReturnError_WhenExistingProductIsUpdated()
		{
			var existingProduct = await AddTestProduct();

			var productService = CreateProductService();

			var errors = await productService.ValidateItem(existingProduct);

			Assert.Empty(errors);
		}

		[Fact]
		public async Task ValidateItem_ShouldAllowSameNameInDifferentStores()
		{
			var storeA = await AddTestStore();
			var storeB = Context.Stores.Add(new Store
			{
				Name = "Foo",
				Description = "Bar"
			});

			var productType = await AddTestProductType();
			var purchaseUnit = await AddTestPurchaseUnit();

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			Context.Products.Add(new Product
			{
				Name = "FooBar",
				StoreId = storeA.Id,
				ProductTypeId = productType.Id,
				PurchaseUnitId = purchaseUnit.Id,
				Picture = [0x01, 0x02, 0x03, 0x04],
				PictureMimeType = "image/jpeg",
				Barcode = [0x05, 0x06, 0x07, 0x08],
				BarcodeMimeType = "image/jpeg"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var newProduct = new Product
			{
				Name = "FooBar",
				StoreId = storeB.Entity.Id,
				ProductTypeId = productType.Id,
				PurchaseUnitId = purchaseUnit.Id,
				Picture = [0x01, 0x02, 0x03, 0x04],
				PictureMimeType = "image/jpeg",
				Barcode = [0x05, 0x06, 0x07, 0x08],
				BarcodeMimeType = "image/jpeg"
			};

			var productService = CreateProductService();

			var errors = await productService.ValidateItem(newProduct);

			Assert.Empty(errors);
		}

		#endregion

	}
}
