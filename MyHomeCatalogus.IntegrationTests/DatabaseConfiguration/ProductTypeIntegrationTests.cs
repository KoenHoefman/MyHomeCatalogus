using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
	public class ProductTypeIntegrationTests : BaseIntegrationTest
	{
		//FK_product_types_products
		[Fact]
		public async Task ProductType_With_Linked_Products_Cannot_Be_Deleted()
		{
			var testProduct = await AddTestProduct();

			var typeToDelete = await Context.ProductTypes.FindAsync([testProduct.ProductTypeId], TestContext.Current.CancellationToken);

			if (typeToDelete != null)
			{
				Context.ProductTypes.Remove(typeToDelete);
			}

			await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
		}

		//UQ_product_types_name
		[Fact]
		public async Task? ProductTypes_Must_Have_A_Uinque_Name()
		{
			var testProductType = await AddTestProductType();

			var duplicateType = new ProductType()
			{
				Name = testProductType.Name,
				Description = "Duplicate name"
			};

			Context.ProductTypes.Add(duplicateType);

			await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
		}

	}
}
