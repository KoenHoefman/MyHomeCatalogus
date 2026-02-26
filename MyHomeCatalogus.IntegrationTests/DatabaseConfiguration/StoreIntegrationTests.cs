using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
    public class StoreIntegrationTests:BaseIntegrationTest
    {
        //UQ_stores_name
        [Fact]
        public async Task Store_Name_Cannot_Be_Duplicated()
        {
            var testStore = await AddTestStore();

            var duplicateStore = new Store
            {
                Name = testStore.Name, 
                Description = "Another Store A"
            };
            Context.Stores.Add(duplicateStore);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        //FK_stores_products
        [Fact]
        public async Task Store_With_Linked_Products_Cannot_Be_Deleted()
        {
            var testProduct = await AddTestProduct();

            var storeToDelete = await Context.Stores.FindAsync([testProduct.StoreId], TestContext.Current.CancellationToken);

            if (storeToDelete != null)
            {
                Context.Stores.Remove(storeToDelete);
            }

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        //FK_stores_shoppinglists
        [Fact]
        public async Task Store_With_Linked_ShoppingLists_Cannot_Be_Deleted()
        {
            var testShoppingList = await AddTestShoppingList();

            var storeToDelete = await Context.Stores.FindAsync([testShoppingList.StoreId], TestContext.Current.CancellationToken);

            if (storeToDelete != null)
            {
                Context.Stores.Remove(storeToDelete);
            }

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

    }
}
