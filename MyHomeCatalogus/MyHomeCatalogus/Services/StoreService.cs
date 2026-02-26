using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Services;

/// <summary>
/// Provides data management services for <see cref="Store"/> entities.
/// </summary>
public class StoreService : IStoreService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="StoreService"/> class.
    /// </summary>
    /// <param name="contextFactory">The factory used to create <see cref="AppDbContext"/> instances.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="contextFactory"/> is null.</exception>
    public StoreService(IDbContextFactory<AppDbContext> contextFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);

        _contextFactory = contextFactory;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Store>> GetAll()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Stores.ToListAsync();
    }

    /// <inheritdoc />
    /// <exception cref="KeyNotFoundException">Thrown when no store with the specified ID is found.</exception>
    public async Task<Store> Get(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Stores.FindAsync(id)
               ?? throw new KeyNotFoundException($"Store with Id {id} not found");
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    /// <exception cref="UniqueConstraintException">Thrown when the entity violates domain validation or unique constraints.</exception>
    public async Task<Store> Add(Store item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var validationErrors = await ValidateItem(item);

        if (validationErrors.Any())
        {
            throw new UniqueConstraintException("Invalid Store", validationErrors);
        }

        await using var context = await _contextFactory.CreateDbContextAsync();

        var addedEntity = context.Stores.Add(item);

        await context.SaveChangesAsync();

        return addedEntity.Entity;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    /// <exception cref="UniqueConstraintException">Thrown when the updated entity violates domain validation or unique constraints.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the store does not exist in the database.</exception>
    public async Task<Store> Update(Store item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var validationErrors = await ValidateItem(item);

        if (validationErrors.Any())
        {
            throw new UniqueConstraintException("Invalid Store", validationErrors);
        }

        await using var context = await _contextFactory.CreateDbContextAsync();

        var foundEntity = await context.Stores.FindAsync(item.Id);

        if (foundEntity is not null)
        {
            context.Entry(foundEntity).CurrentValues.SetValues(item);

            await context.SaveChangesAsync();
        }

        return foundEntity ?? throw new KeyNotFoundException($"Store with Id {item.Id} not found");
    }

    /// <inheritdoc />
    /// <remarks>This operation is idempotent; if the ID does not exist, the method completes without error.</remarks>
    public async Task Delete(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var foundEntity = await context.Stores.FirstOrDefaultAsync(p => p.Id == id);

        if (foundEntity == null)
        {
            return;
        }

        context.Stores.Remove(foundEntity);

        await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<List<(string PropertyName, string ErrorMessage)>> ValidateItem(Store item)
    {
        var returnValue = new List<(string PropertyName, string ErrorMessage)>();

        await using var context = await _contextFactory.CreateDbContextAsync();

        //Unique index on name
        var duplicate = await context.Stores
            .AnyAsync(s => s.Name == item.Name && s.Id != item.Id);

        if (duplicate)
        {
            returnValue.Add((nameof(item.Name), "A store with this name already exists."));
        }

        return returnValue;
    }
}