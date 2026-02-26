using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
    public class ShoppingListIntegrationTests:BaseIntegrationTest
    {
        //UQ_shoppinglists_store_id_one_active_list
        [Fact]
        public async Task ShoppingList_Only_One_Active_List_Per_Store_Is_Allowed()
        {
            // TestShoppingList is incomplete (IsCompleted = false)
            var testShoppingList = await AddTestShoppingList();

            // Attempt to create a second incomplete list for StoreId = 1
            var secondActiveList = new ShoppingList
            {
                StoreId = testShoppingList.StoreId,
                IsCompleted = false, // Duplicate state
                DateCreated = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
            };

            Context.ShoppingLists.Add(secondActiveList);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        //UQ_shoppinglists_store_id_one_active_list
        [Fact]
        public async Task ShoppingList_Multiple_Completed_Lists_Are_Allowed()
        {
            //Complete the first list
            var testShoppingList = await AddTestShoppingList();

            var list1 = await Context.ShoppingLists.FindAsync([testShoppingList.Id], TestContext.Current.CancellationToken);

            if (list1 != null)
            {
                list1.IsCompleted = true;
            }

            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
            Context.ChangeTracker.Clear();

            // Create a new completed list for same store
            var secondCompletedList = new ShoppingList
            {
                StoreId = testShoppingList.StoreId,
                IsCompleted = true,
                DateCreated = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
            };

            Context.ShoppingLists.Add(secondCompletedList);

            var result = await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task ShoppingList_For_Store_Can_Have_New_Uncompleted_List_If_Others_Are_Completed()
        {
            //Complete the first list
            var testShoppingList = await AddTestShoppingList();

            var list1 = await Context.ShoppingLists.FindAsync([testShoppingList.Id], TestContext.Current.CancellationToken);

            if (list1 != null)
            {
                list1.IsCompleted = true;
            }

            await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
            Context.ChangeTracker.Clear();

            // Create a new completed list for same store
            var secondCompletedList = new ShoppingList
            {
                StoreId = testShoppingList.StoreId,
                IsCompleted = false,
                DateCreated = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
            };

            Context.ShoppingLists.Add(secondCompletedList);

            var result = await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            Assert.Equal(1, result);
        }

        //FK_shoppinglist_items_shoppinglists
        [Fact]
        public async Task ShoppingList_With_Linked_Items_Cannot_Be_Deleted()
        {
            var testShoppingListItem = await AddTestShoppingListItem();

            var listToDelete = await Context.ShoppingLists.FindAsync([testShoppingListItem.ShoppingListId], TestContext.Current.CancellationToken);
            
            if (listToDelete != null)
            {
                Context.ShoppingLists.Remove(listToDelete);
            }

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ShoppingList_AutoInclude_NaviagationProperties()
        {
            var addedEntity = await AddTestShoppingList();

            var result = await Context.ShoppingLists
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == addedEntity.Id, TestContext.Current.CancellationToken);

            Assert.NotNull(result);

            //AutoInclude

            //Not included
            Assert.Null(result.Store);
            Assert.NotNull(result.ShoppingListItems);
            Assert.Empty(result.ShoppingListItems);
        }

    }
}
