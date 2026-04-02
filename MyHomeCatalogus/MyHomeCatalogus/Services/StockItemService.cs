using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Services;

/// <summary>
/// Provides data management services for <see cref="StockItem"/> entities.
/// </summary>
public class StockItemService : IStockItemService
{
	private readonly IDbContextFactory<AppDbContext> _contextFactory;
	private readonly ILogger<StockItemService> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="StockItemService"/> class.
	/// </summary>
	/// <param name="contextFactory">The factory used to create <see cref="AppDbContext"/> instances.</param>
	/// <param name="logger">The logger for logging errors.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="contextFactory"/> is null.</exception>
	public StockItemService(IDbContextFactory<AppDbContext> contextFactory, ILogger<StockItemService> logger)
	{
		ArgumentNullException.ThrowIfNull(contextFactory);
		ArgumentNullException.ThrowIfNull(logger);

		_contextFactory = contextFactory;
		_logger = logger;
	}


	/// <inheritdoc />
	public async Task<IEnumerable<StockItem>> GetAll()
	{
		try
		{
			//ToDo: Add filtering / sorting / paging
			await using var context = await _contextFactory.CreateDbContextAsync();

			return await context.StockItems.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving all stock items.");
			throw;
		}
	}

	/// <inheritdoc />
	/// <exception cref="KeyNotFoundException">Thrown when no stock item with the specified ID is found.</exception>
	public async Task<StockItem> Get(int id)
	{
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			return await context.StockItems.FindAsync(id)
				   ?? throw new KeyNotFoundException($"StockItem with Id {id} not found");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting stock item with Id {StockItemId}.", id);
			throw;
		}
	}

	/// <inheritdoc />
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
	/// <exception cref="UniqueConstraintException">Thrown when the entity violates domain validation or unique constraints.</exception>
	public async Task<StockItem> Add(StockItem item)
	{
		ArgumentNullException.ThrowIfNull(item);

		var validationErrors = await ValidateItem(item);

		if (validationErrors.Any())
		{
			throw new UniqueConstraintException("Invalid Stockitem", validationErrors);
		}

		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			var addedEntity = context.StockItems.Add(item);

			await context.SaveChangesAsync();

			return addedEntity.Entity;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error adding stock item.");
			throw;
		}
	}

	/// <inheritdoc />
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
	/// <exception cref="UniqueConstraintException">Thrown when the updated entity violates domain validation or unique constraints.</exception>
	/// <exception cref="KeyNotFoundException">Thrown when the stock item does not exist in the database.</exception>
	public async Task<StockItem> Update(StockItem item)
	{
		ArgumentNullException.ThrowIfNull(item);

		var validationErrors = await ValidateItem(item);

		if (validationErrors.Any())
		{
			throw new UniqueConstraintException("Invalid Stockitem", validationErrors);
		}

		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			var foundEntity = await context.StockItems.FindAsync(item.Id);

			if (foundEntity is not null)
			{
				context.Entry(foundEntity).CurrentValues.SetValues(item);

				await context.SaveChangesAsync();
			}

			return foundEntity ?? throw new KeyNotFoundException($"StockItem with Id {item.Id} not found");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating stock item with Id {StockItemId}.", item.Id);
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

			var foundEntity = await context.StockItems.FirstOrDefaultAsync(p => p.Id == id);

			if (foundEntity == null)
			{
				return;
			}

			//ToDo: implement soft delete to keep the audit history.
			var audits = await context.StockItemAudits.Where(p => p.StockItemId == id).ToListAsync();

			if (audits.Any())
			{
				context.StockItemAudits.RemoveRange(audits);
			}

			context.StockItems.Remove(foundEntity);

			await context.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting stock item with Id {StockItemId}.", id);
			throw;
		}
	}

	/// <inheritdoc />
	public async Task<List<(string PropertyName, string ErrorMessage)>> ValidateItem(StockItem item)
	{
		try
		{
			var returnValue = new List<(string PropertyName, string ErrorMessage)>();

			await using var context = await _contextFactory.CreateDbContextAsync();

			//Unique index on product and location
			var duplicate = await context.StockItems
				.AnyAsync(p => p.ProductId == item.ProductId && p.ShelfId == item.ShelfId && p.Id != item.Id);

			if (duplicate)
			{
				returnValue.Add((nameof(item.ProductId), "This product is already on this location.."));
			}

			return returnValue;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error validating stock item.");
			throw;
		}
	}
}
