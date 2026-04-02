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
	private readonly ILogger<ProductThresholdService> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="ProductThresholdService"/> class.
	/// </summary>
	/// <param name="contextFactory">The factory used to create <see cref="AppDbContext"/> instances.</param>
	/// <param name="logger">The logger for logging errors.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="contextFactory"/> is null.</exception>
	public ProductThresholdService(IDbContextFactory<AppDbContext> contextFactory, ILogger<ProductThresholdService> logger)
	{
		ArgumentNullException.ThrowIfNull(contextFactory);
		ArgumentNullException.ThrowIfNull(logger);

		_contextFactory = contextFactory;
		_logger = logger;
	}

	/// <inheritdoc />
	public async Task<IEnumerable<ProductThreshold>> GetAll()
	{
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			return await context.ProductThresholds.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving all product thresholds.");
			throw;
		}
	}

	/// <inheritdoc />
	/// <exception cref="KeyNotFoundException">Thrown when no threshold with the specified ID exists.</exception>
	public async Task<ProductThreshold> Get(int id)
	{
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			return await context.ProductThresholds.FindAsync(id)
				   ?? throw new KeyNotFoundException($"ProductThreshold with Id {id} not found");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving product threshold with Id {ThresholdId}.", id);
			throw;
		}
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

		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			var addedEntity = context.ProductThresholds.Add(item);

			await context.SaveChangesAsync();

			return addedEntity.Entity;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error adding product threshold.");
			throw;
		}
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

		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			var foundEntity = await context.ProductThresholds.FindAsync(item.Id);

			if (foundEntity is not null)
			{
				context.Entry(foundEntity).CurrentValues.SetValues(item);
				await context.SaveChangesAsync();
			}

			return foundEntity ?? throw new KeyNotFoundException($"Productthreshold with Id {item.Id} not found");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating product threshold with Id {ThresholdId}.", item.Id);
			throw;
		}
	}

	/// <inheritdoc />
	/// <remarks>The operation is idempotent; if the record is not found, the method exits silently.</remarks>
	public async Task Delete(int id)
	{
		try
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
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting product threshold with Id {ThresholdId}.", id);
			throw;
		}
	}

	/// <inheritdoc />
	public async Task<List<(string PropertyName, string ErrorMessage)>> ValidateItem(ProductThreshold item)
	{
		try
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
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error validating product threshold.");
			throw;
		}
	}
}
