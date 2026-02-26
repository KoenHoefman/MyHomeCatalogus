using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
    public class ShoppingListItemIntegrationTests : BaseIntegrationTest
    {
        //UQ_shoppinglist_items_shoppinglist_id_product_id
        [Fact]
        public async Task ShoppingListItem_Same_Product_Cannot_Be_Duplicated_In_Same_List()
        {
            var testItem = await AddTestShoppingListItem();

            var duplicateItem = new ShoppingListItem
            {
                ShoppingListId = testItem.ShoppingListId,
                ProductId = testItem.ProductId,
                Quantity = 10
            };

            Context.ShoppingListItems.Add(duplicateItem);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        //CK_shoppinglist_items_quantity_positive
        [Fact]
        public async Task ShoppingListItem_Quantity_Must_Be_Positive()
        {
            var testProduct = await AddTestProduct();

            var testShoppingList = Context.ShoppingLists.Add(new ShoppingList
            {
                StoreId = testProduct.StoreId,
                IsCompleted = false,
                DateCreated = DateOnly.FromDateTime(DateTime.Today)
            });

            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
            Context.ChangeTracker.Clear();

            var item = new ShoppingListItem
            {
                ShoppingListId = testShoppingList.Entity.Id,
                ProductId = testProduct.Id,
                Quantity = 0
            };

            Context.ShoppingListItems.Add(item);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ShoppingListItem_AutoInclude_NaviagationProperties()
        {
            var addedEntity = await AddTestShoppingListItem();

            var result = await Context.ShoppingListItems
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == addedEntity.Id, TestContext.Current.CancellationToken);

            Assert.NotNull(result);

            //AutoInclude

            //Not included
            Assert.Null(result.ShoppingList);
            Assert.Null(result.Product);
        }

    }
}
