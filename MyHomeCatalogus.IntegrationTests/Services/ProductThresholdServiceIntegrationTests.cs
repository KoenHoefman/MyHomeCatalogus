using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Services;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.Services
{
	public class ProductThresholdServiceIntegrationTests : BaseIntegrationTest
	{
		private ProductThresholdService CreateProductThresholdService()
		{
			var contextFactory = new DbContextFactoryMock(Options, Context.Database.GetDbConnection());
			return new ProductThresholdService(contextFactory, NullLogger<ProductThresholdService>.Instance);
		}

		#region GetAll

		[Fact]
		public async Task GetAll_ShouldReturnEmpty_WhenNoThresholdsExist()
		{
			var service = CreateProductThresholdService();

			var result = await service.GetAll();

			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAll_ShouldReturnAllThresholds()
		{
			var product1 = await AddTestProduct();

			var product2 = Context.Products.Add(new Product
			{
				Name = "Foo",
				ProductTypeId = product1.ProductTypeId,
				StoreId = product1.StoreId,
				PurchaseUnitId = product1.PurchaseUnitId,
				Picture = [0x01, 0x02, 0x03, 0x04],
				PictureMimeType = "image/jpeg",
				Barcode = [0x05, 0x06, 0x07, 0x08],
				BarcodeMimeType = "image/jpeg"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			Context.ProductThresholds.Add(new ProductThreshold
			{
				ProductId = product1.Id,
				Threshold = 5,
				PurchaseQuantity = 2
			});

			Context.ProductThresholds.Add(new ProductThreshold
			{
				ProductId = product2.Entity.Id,
				Threshold = 10,
				PurchaseQuantity = 5
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var service = CreateProductThresholdService();

			var result = await service.GetAll();

			Assert.Equal(2, result.Count());
		}

		#endregion

		#region Get

		[Fact]
		public async Task Get_ShouldThrowKeyNotFoundException_WhenIdDoesNotExist()
		{
			var service = CreateProductThresholdService();

			await Assert.ThrowsAsync<KeyNotFoundException>(() => service.Get(999));
		}

		[Fact]
		public async Task Get_ShouldReturnThreshold_WhenIdExists()
		{
			var threshold = await AddTestProductThreshold();

			var service = CreateProductThresholdService();

			var result = await service.Get(threshold.Id);

			Assert.Equal(threshold.Id, result.Id);
			Assert.Equal(threshold.Threshold, result.Threshold);
			Assert.Equal(threshold.ProductId, result.ProductId);
			Assert.Equal(threshold.PurchaseQuantity, result.PurchaseQuantity);
		}

		#endregion

		#region Add

		[Fact]
		public async Task Add_ShouldPersistThreshold_WhenValid()
		{
			var product = await AddTestProduct();

			var newThreshold = new ProductThreshold
			{
				ProductId = product.Id,
				Threshold = 5,
				PurchaseQuantity = 2
			};

			var service = CreateProductThresholdService();

			var result = await service.Add(newThreshold);

			Assert.True(result.Id > 0);

			Context.ChangeTracker.Clear();

			var retrievedProduct = await Context.ProductThresholds.FindAsync([result.Id], TestContext.Current.CancellationToken);

			Assert.NotNull(retrievedProduct);
			Assert.Equal(newThreshold.ProductId, retrievedProduct.ProductId);
			Assert.Equal(newThreshold.Threshold, result.Threshold);
			Assert.Equal(newThreshold.ProductId, result.ProductId);
			Assert.Equal(newThreshold.PurchaseQuantity, result.PurchaseQuantity);
		}

		[Fact]
		public async Task Add_ShouldThrowUniqueConstraintException_WhenProductAlreadyHasThreshold()
		{
			var existingThreshold = await AddTestProductThreshold();

			var duplicate = new ProductThreshold
			{
				ProductId = existingThreshold.ProductId,
				Threshold = 5,
				PurchaseQuantity = 5
			};

			var service = CreateProductThresholdService();

			var ex = await Assert.ThrowsAsync<UniqueConstraintException>(() => service.Add(duplicate));

			Assert.Contains(ex.ValidationErrors, e => e.PropertyName == nameof(ProductThreshold.ProductId));
		}

		#endregion

		#region Update

		[Fact]
		public async Task Update_ShouldModifyExistingThreshold()
		{
			var existingThreshold = await AddTestProductThreshold();

			var newThresholdValue = existingThreshold.Threshold + 5;
			var newPurchaseQuantity = existingThreshold.PurchaseQuantity + 1;

			existingThreshold.Threshold = newThresholdValue;
			existingThreshold.PurchaseQuantity = newPurchaseQuantity;

			var service = CreateProductThresholdService();

			var result = await service.Update(existingThreshold);

			Assert.Equal(newThresholdValue, result.Threshold);

			Context.ChangeTracker.Clear();

			var retrievedProduct = await Context.ProductThresholds.FirstOrDefaultAsync(x => x.Id == existingThreshold.Id, TestContext.Current.CancellationToken);

			Assert.NotNull(retrievedProduct);

			Assert.Equal(newThresholdValue, retrievedProduct.Threshold);
			Assert.Equal(newThresholdValue, retrievedProduct.Threshold);
			Assert.Equal(newPurchaseQuantity, retrievedProduct.PurchaseQuantity);
		}

		#endregion

		#region Delete

		[Fact]
		public async Task Delete_ShouldRemoveThreshold_WhenIdExists()
		{
			var threshold = await AddTestProductThreshold();

			var service = CreateProductThresholdService();

			await service.Delete(threshold.Id);

			Context.ChangeTracker.Clear();

			var retrievedProduct = await Context.ProductThresholds.FirstOrDefaultAsync(t => t.Id == threshold.Id, TestContext.Current.CancellationToken);

			Assert.Null(retrievedProduct);
		}

		[Fact]
		public async Task Delete_ShouldBeIdempotent_WhenIdDoesNotExist()
		{
			var service = CreateProductThresholdService();

			var exception = await Record.ExceptionAsync(() => service.Delete(999));

			Assert.Null(exception);
		}

		#endregion

		#region ValidateItem

		[Fact]
		public async Task ValidateItem_ShouldReturnError_WhenProductIsDuplicate()
		{
			var existingThreshold = await AddTestProductThreshold();

			var newItem = new ProductThreshold
			{
				ProductId = existingThreshold.ProductId,
				Threshold = 5,
				PurchaseQuantity = 5
			};

			var service = CreateProductThresholdService();

			var errors = await service.ValidateItem(newItem);

			var error = Assert.Single(errors);
			Assert.Equal(nameof(ProductThreshold.ProductId), error.PropertyName);
			Assert.Equal("There is already a threshold for this product.", error.ErrorMessage);
		}

		#endregion
	}
}
