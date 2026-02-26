using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Services;

/// <summary>
/// Service for managing <see cref="ProductThreshold"/> entities, handling stock level alerts and domain validation.
/// </summary>
public class ProductThresholdService : IProductThresholdService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductThresholdService"/> class.
    /// </summary>
    /// <param name="contextFactory">The factory used to create <see cref="AppDbContext"/> instances.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="contextFactory"/> is null.</exception>
    public ProductThresholdService(IDbContextFactory<AppDbContext> contextFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);

        _contextFactory = contextFactory;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProductThreshold>> GetAll()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.ProductThresholds.ToListAsync();
    }

    /// <inheritdoc />
    /// <exception cref="KeyNotFoundException">Thrown when no threshold with the specified ID exists.</exception>
    public async Task<ProductThreshold> Get(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.ProductThresholds.FindAsync(id)
               ?? throw new KeyNotFoundException($"ProductThreshold with Id {id} not found");
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    /// <exception cref="UniqueConstraintException">Thrown when the entity violates domain validation or unique constraints.</exception>
    public async Task<ProductThreshold> Add(ProductThreshold item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var validationErrors = await ValidateItem(item);

        if (validationErrors.Any())
        {
            throw new UniqueConstraintException("Invalid ProductThreshold", validationErrors);
        }

        await using var context = await _contextFactory.CreateDbContextAsync();

        var addedEntity = context.ProductThresholds.Add(item);

        await context.SaveChangesAsync();

        return addedEntity.Entity;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    /// <exception cref="UniqueConstraintException">Thrown when the updated entity violates domain validation or unique constraints.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the threshold to update is not found.</exception>
    public async Task<ProductThreshold> Update(ProductThreshold item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var validationErrors = await ValidateItem(item);

        if (validationErrors.Any())
        {
            throw new UniqueConstraintException("Invalid ProductThreshold", validationErrors);
        }

        await using var context = await _contextFactory.CreateDbContextAsync();

        var foundEntity = await context.ProductThresholds.FindAsync(item.Id);

        if (foundEntity is not null)
        {
            context.Entry(foundEntity).CurrentValues.SetValues(item);

            await context.SaveChangesAsync();
        }

        return foundEntity ?? throw new KeyNotFoundException($"Productthreshold with Id {item.Id} not found");
    }

    /// <inheritdoc />
    /// <remarks>The operation is idempotent; if the record is not found, the method exits silently.</remarks>
    public async Task Delete(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var foundEntity = await context.ProductThresholds.FirstOrDefaultAsync(p => p.Id == id);

        if (foundEntity == null)
        {
            return;
        }

        context.ProductThresholds.Remove(foundEntity);

        await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<List<(string PropertyName, string ErrorMessage)>> ValidateItem(ProductThreshold item)
    {
        var returnValue = new List<(string PropertyName, string ErrorMessage)>();

        await using var context = await _contextFactory.CreateDbContextAsync();

        //Only 1 threshold per product allowed
        var duplicate = await context.ProductThresholds
            .AnyAsync(s => s.ProductId == item.ProductId && s.Id != item.Id);

        if (duplicate)
        {
            returnValue.Add((nameof(item.ProductId), "There is already a threshold for this product."));
        }

        return returnValue;
    }
}