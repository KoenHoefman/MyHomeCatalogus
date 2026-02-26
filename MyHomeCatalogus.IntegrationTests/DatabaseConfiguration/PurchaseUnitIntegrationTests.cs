using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
    public class PurchaseUnitIntegrationTests:BaseIntegrationTest
    {
        //FK_purchaseunits_products
        [Fact]
        public async Task PurchaseUnit_With_Linked_Products_Cannot_Be_Deleted()
        {
            var testProduct = await AddTestProduct();

            var purchaseUnitToDelete = await Context.PurchaseUnits.FindAsync([testProduct.PurchaseUnitId], TestContext.Current.CancellationToken);

            if (purchaseUnitToDelete != null)
            {
                Context.PurchaseUnits.Remove(purchaseUnitToDelete);
            }

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        //UQ_purchaseunits_name
        [Fact]
        public async Task? PurchaseUnits_Must_Have_A_Uinque_Name()
        {
            var testPurchaseUnit = await AddTestPurchaseUnit();

            var duplicatePurchaseUnit = new PurchaseUnit()
            {
                Name = testPurchaseUnit.Name
            };

             Context.PurchaseUnits.Add(duplicatePurchaseUnit);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

    }
}
