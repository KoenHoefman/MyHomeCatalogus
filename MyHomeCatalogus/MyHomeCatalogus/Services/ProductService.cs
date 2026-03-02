using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;
using System.Linq.Expressions;

namespace MyHomeCatalogus.Services;

/// <summary>
/// Provides data management services for <see cref="Product"/> entities.
/// </summary>
public class ProductService : IProductService
{
	private readonly IDbContextFactory<AppDbContext> _contextFactory;

	/// <summary>
	/// Initializes a new instance of the <see cref="ProductService"/> class.
	/// </summary>
	/// <param name="contextFactory">The factory used to create <see cref="AppDbContext"/> instances.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="contextFactory"/> is null.</exception>
	public ProductService(IDbContextFactory<AppDbContext> contextFactory)
	{
		ArgumentNullException.ThrowIfNull(contextFactory);
		_contextFactory = contextFactory;
	}

	/// <summary>
	/// Retrieves all products that match the specified filter expression.
	/// </summary>
	/// <param name="filter">A lambda expression used to filter the products.</param>
	/// <returns>A collection of filtered products.</returns>
	public async Task<IEnumerable<Product>> GetAll(Expression<Func<Product, bool>>? filter)
	{
		await using var context = await _contextFactory.CreateDbContextAsync();
		IQueryable<Product> query = context.Products;

		if (filter != null)
		{
			query = query.Where(filter);
		}

		return await query.ToListAsync();
	}

	/// <inheritdoc />
	public async Task<IEnumerable<Product>> GetAll()
	{
		await using var context = await _contextFactory.CreateDbContextAsync();
		return await context.Products.ToListAsync();
	}

	/// <inheritdoc />
	/// <exception cref="KeyNotFoundException">Thrown when no product with the specified ID is found.</exception>
	public async Task<Product> Get(int id)
	{
		await using var context = await _contextFactory.CreateDbContextAsync();
		return await context.Products.FindAsync(id)
			   ?? throw new KeyNotFoundException($"Product with Id {id} not found");
	}

	/// <inheritdoc />
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
	/// <exception cref="UniqueConstraintException">Thrown when the entity violates domain validation or unique constraints.</exception>
	public async Task<Product> Add(Product item)
	{
		ArgumentNullException.ThrowIfNull(item);

		var validationErrors = await ValidateItem(item);
		if (validationErrors.Any())
		{
			throw new UniqueConstraintException("Invalid Product", validationErrors);
		}

		await using var context = await _contextFactory.CreateDbContextAsync();
		var addedEntity = context.Products.Add(item);
		await context.SaveChangesAsync();

		return addedEntity.Entity;
	}

	/// <inheritdoc />
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
	/// <exception cref="UniqueConstraintException">Thrown when the updated entity violates domain validation or unique constraints.</exception>
	/// <exception cref="KeyNotFoundException">Thrown when the product does not exist in the database.</exception>
	public async Task<Product> Update(Product item)
	{
		ArgumentNullException.ThrowIfNull(item);

		var validationErrors = await ValidateItem(item);
		if (validationErrors.Any())
		{
			throw new UniqueConstraintException("Invalid Product", validationErrors);
		}

		await using var context = await _contextFactory.CreateDbContextAsync();
		var foundEntity = await context.Products.FindAsync(item.Id);

		if (foundEntity is not null)
		{
			context.Entry(foundEntity).CurrentValues.SetValues(item);
			await context.SaveChangesAsync();
		}

		return foundEntity ?? throw new KeyNotFoundException($"Product with Id {item.Id} not found");
	}

	/// <inheritdoc />
	/// <remarks>This operation is idempotent; if the ID does not exist, the method completes without error.</remarks>
	public async Task Delete(int id)
	{
		await using var context = await _contextFactory.CreateDbContextAsync();
		var foundEntity = await context.Products.FirstOrDefaultAsync(p => p.Id == id);

		if (foundEntity == null) return;

		context.Products.Remove(foundEntity);
		await context.SaveChangesAsync();
	}

	/// <inheritdoc />
	public async Task<List<(string PropertyName, string ErrorMessage)>> ValidateItem(Product item)
	{
		var returnValue = new List<(string PropertyName, string ErrorMessage)>();
		await using var context = await _contextFactory.CreateDbContextAsync();

		//Unique index on name per store
		var duplicate = await context.Products
			.AnyAsync(p => p.Name == item.Name && p.StoreId == item.StoreId && p.Id != item.Id);

		if (duplicate)
		{
			returnValue.Add((nameof(item.Name), "A product with this name already exists for this store."));
		}

		return returnValue;
	}

}