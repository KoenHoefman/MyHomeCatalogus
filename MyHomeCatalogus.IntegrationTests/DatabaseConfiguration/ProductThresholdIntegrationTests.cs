using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
    public class ProductThresholdIntegrationTests : BaseIntegrationTest
    {
        //CK_productthresholds_purchasequantity_positive
        [Fact]
        public async Task ProductThreshold_PurchaseQuantity_Must_Be_Positive()
        {
            var invalidThreshold = new ProductThreshold
            {
                ProductId = (await AddTestProduct()).Id,
                Threshold = 1,
                PurchaseQuantity = 0
            };

             Context.ProductThresholds.Add(invalidThreshold);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        //CK_productthresholds_threshold_zero_or_positive
        [Fact]
        public async Task ProductThreshold_Threshold_Cannot_Be_Negative()
        {
            var invalidThreshold = new ProductThreshold
            {
                ProductId = (await AddTestProduct()).Id,
                Threshold = -1,
                PurchaseQuantity = 1
            };

             Context.ProductThresholds.Add(invalidThreshold);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        //CK_productthresholds_threshold_zero_or_positive
        [Fact]
        public async Task ProductThreshold_Quantity_Can_Be_Zero()
        {
            var validThreshold = new ProductThreshold
            {
                ProductId = (await AddTestProduct()).Id,
                Threshold = 0,
                PurchaseQuantity = 1
            };

             Context.ProductThresholds.Add(validThreshold);

            var result = await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            Assert.Equal(1, result);
        }

        //UQ_product_thresholds_product_id
        [Fact]
        public async Task ProductThreshold_Same_Product_Cannot_Be_Duplicated()
        {
            // Add a valid item first
            var validThreshold = new ProductThreshold
            {
                ProductId = (await AddTestProduct()).Id,
                Threshold = 1,
                PurchaseQuantity = 1
            };

             Context.ProductThresholds.Add(validThreshold);

            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
            Context.ChangeTracker.Clear();

            // Attempt to add a duplicate (same product)
            var duplicateThreshold = new ProductThreshold
            {
                ProductId = validThreshold.ProductId,
                Threshold = 3,
                PurchaseQuantity = 5
            };

             Context.ProductThresholds.Add(duplicateThreshold);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ProductThreshold_AutoInclude_NaviagationProperties()
        {
            var addedEntity = await AddTestProductThreshold();

            var result = await Context.ProductThresholds
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == addedEntity.Id, TestContext.Current.CancellationToken);

            Assert.NotNull(result);

            //AutoInclude
            Assert.NotNull(result.Product);
        }
    }
}
