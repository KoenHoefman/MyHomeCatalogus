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
	private readonly ILogger<ProductService> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="ProductService"/> class.
	/// </summary>
	/// <param name="contextFactory">The factory used to create <see cref="AppDbContext"/> instances.</param>
	/// <param name="logger">The logger for logging errors.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="contextFactory"/> is null.</exception>
	public ProductService(IDbContextFactory<AppDbContext> contextFactory, ILogger<ProductService> logger)
	{
		ArgumentNullException.ThrowIfNull(contextFactory);
		ArgumentNullException.ThrowIfNull(logger);
		_contextFactory = contextFactory;
		_logger = logger;
	}

	/// <summary>
	/// Retrieves all products that match the specified filter expression.
	/// </summary>
	/// <param name="filter">A lambda expression used to filter the products.</param>
	/// <returns>A collection of filtered products.</returns>
	public async Task<IEnumerable<Product>> GetAll(Expression<Func<Product, bool>>? filter)
	{
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();
			IQueryable<Product> query = context.Products;

			if (filter != null)
			{
				query = query.Where(filter);
			}

			return await query.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving filtered products.");
			throw;
		}
	}

	/// <inheritdoc />
	public async Task<IEnumerable<Product>> GetAll()
	{
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();
			return await context.Products.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving all products.");
			throw;
		}
	}

	/// <inheritdoc />
	/// <exception cref="KeyNotFoundException">Thrown when no product with the specified ID is found.</exception>
	public async Task<Product> Get(int id)
	{
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();
			return await context.Products.FindAsync(id)
				   ?? throw new KeyNotFoundException($"Product with Id {id} not found");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving product with Id {ProductId}.", id);
			throw;
		}
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

		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();
			var addedEntity = context.Products.Add(item);

			await context.SaveChangesAsync();
			return addedEntity.Entity;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error saving new product.");
			throw;
		}
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

		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();
			var foundEntity = await context.Products.FindAsync(item.Id);

			if (foundEntity is not null)
			{
				context.Entry(foundEntity).CurrentValues.SetValues(item);
				await context.SaveChangesAsync();
			}

			return foundEntity ?? throw new KeyNotFoundException($"Product with Id {item.Id} not found");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating product with Id {ProductId}.", item.Id);
			throw;
		}
	}

	/// <inheritdoc />
	/// <remarks>This operation is idempotent; if the ID does not exist, the method completes without error.</remarks>
	public async Task Delete(int id)
	{
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();
			var foundEntity = await context.Products.FirstOrDefaultAsync(p => p.Id == id);

			if (foundEntity == null)
			{
				return;
			}

			context.Products.Remove(foundEntity);
			await context.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting product with Id {ProductId}.", id);
			throw;
		}
	}

	/// <inheritdoc />
	public async Task<List<(string PropertyName, string ErrorMessage)>> ValidateItem(Product item)
	{
		try
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
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error validating product.");
			throw;
		}
	}

}
