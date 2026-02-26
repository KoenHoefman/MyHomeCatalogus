using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;
using System;

namespace MyHomeCatalogus.Services;

/// <summary>
/// Provides data management services for <see cref="ShoppingListItem"/> entities.
/// </summary>
public class ShoppingListItemService : IShoppingListItemService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShoppingListItemService"/> class.
    /// </summary>
    /// <param name="contextFactory">The factory used to create <see cref="AppDbContext"/> instances.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="contextFactory"/> is null.</exception>
    public ShoppingListItemService(IDbContextFactory<AppDbContext> contextFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);

        _contextFactory = contextFactory;
    }

    /// <summary>
    /// Retrieves all items belonging to a specific shopping list, including product details.
    /// </summary>
    /// <param name="shoppingListId">The unique identifier of the shopping list.</param>
    /// <returns>A collection of shopping list items for the specified list.</returns>
    public async Task<IEnumerable<ShoppingListItem>> GetAllItemsForShoppingList(int shoppingListId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.ShoppingListItems.Where(s => s.ShoppingListId == shoppingListId).Include(s => s.Product).ToListAsync();
    }

    /// <inheritdoc />
    /// <exception cref="KeyNotFoundException">Thrown when the product ID does not exist.</exception>
    /// <exception cref="UniqueConstraintException">Thrown when the item violates domain validation.</exception>
    public async Task<ShoppingListItem> AddProduct(int productId, int quantity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var product = await context.Products.FindAsync(productId)
                             ?? throw new KeyNotFoundException($"Product with Id {productId} not found");

        var shoppingList = await context.ShoppingLists
            .SingleOrDefaultAsync(s => s.StoreId == product.StoreId && !s.IsCompleted);

        if (shoppingList is null)
        {
            //Create new shoppinglist for this store
            shoppingList = new ShoppingList
            {
                StoreId = product.StoreId
            };

            context.ShoppingLists.Add(shoppingList);
            await context.SaveChangesAsync();
        }

        var item = new ShoppingListItem
        { ProductId = productId, ShoppingListId = shoppingList.Id, Quantity = quantity };

        var validationErrors = await ValidateItem(item);

        if (validationErrors.Any())
        {
            throw new UniqueConstraintException("Invalid ShoppingListItem", validationErrors);
        }

        var addedEntity = context.ShoppingListItems.Add(item);

        await context.SaveChangesAsync();

        return addedEntity.Entity;
    }

    /// <inheritdoc />
    /// <exception cref="NotImplementedException">Thrown because items should be retrieved via <see cref="GetAllItemsForShoppingList"/>.</exception>
    public Task<IEnumerable<ShoppingListItem>> GetAll()
    {
        throw new NotImplementedException("Use GetAllItemsForShoppingList.");
    }

    /// <inheritdoc />
    /// <exception cref="KeyNotFoundException">Thrown when no shopping list item with the specified ID is found.</exception>
    public async Task<ShoppingListItem> Get(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.ShoppingListItems.SingleOrDefaultAsync(s => s.Id == id)
               ?? throw new KeyNotFoundException($"ShoppingListItem with Id {id} not found");
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    /// <exception cref="UniqueConstraintException">Thrown when the entity violates domain validation or unique constraints.</exception>
    public async Task<ShoppingListItem> Add(ShoppingListItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var validationErrors = await ValidateItem(item);

        if (validationErrors.Any())
        {
            throw new UniqueConstraintException("Invalid ShoppingListItem", validationErrors);
        }

        await using var context = await _contextFactory.CreateDbContextAsync();

        var addedEntity = context.ShoppingListItems.Add(item);

        await context.SaveChangesAsync();

        return addedEntity.Entity;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    /// <exception cref="UniqueConstraintException">Thrown when the updated entity violates domain validation or unique constraints.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the shopping list item does not exist in the database.</exception>
    public async Task<ShoppingListItem> Update(ShoppingListItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var validationErrors = await ValidateItem(item);

        if (validationErrors.Any())
        {
            throw new UniqueConstraintException("Invalid ShoppingListItem", validationErrors);
        }

        await using var context = await _contextFactory.CreateDbContextAsync();

        var foundEntity = await context.ShoppingListItems.FindAsync(item.Id);

        if (foundEntity is not null)
        {
            context.Entry(foundEntity).CurrentValues.SetValues(item);

            await context.SaveChangesAsync();
        }

        return foundEntity ?? throw new KeyNotFoundException($"ShoppingListItem with Id {item.Id} not found");
    }

    /// <inheritdoc />
    /// <remarks>This operation is idempotent; if the ID does not exist, the method completes without error.</remarks>
    public async Task Delete(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var foundEntity = await context.ShoppingListItems.FirstOrDefaultAsync(p => p.Id == id);

        if (foundEntity == null)
        {
            return;
        }

        context.ShoppingListItems.Remove(foundEntity);

        await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<List<(string PropertyName, string ErrorMessage)>> ValidateItem(ShoppingListItem item)
    {
        var returnValue = new List<(string PropertyName, string ErrorMessage)>();

        await using var context = await _contextFactory.CreateDbContextAsync();

        //Quantity should be greater than 0
        if (item.Quantity < 1)
        {
            returnValue.Add((nameof(item.Quantity), "Quantity must be greater than 0."));
        }

        //Product only once per list
        var duplicate = await context.ShoppingListItems
            .AnyAsync(s => s.ProductId == item.ProductId && s.Id != item.Id);

        if (duplicate)
        {
            returnValue.Add((nameof(item.ProductId), "This product is already on the shoppinglist."));
        }

        //Store should be the same for list and product
        var product = await context.Products.FindAsync(item.ProductId);
        var shoppingList = await context.ShoppingLists.FindAsync(item.ShoppingListId);

        if (product!.StoreId != shoppingList!.StoreId)
        {
            returnValue.Add((nameof(item.ShoppingListId), "The product is from a different store compared to the shoppinglist."));
        }

        return returnValue;
    }
}