using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Services;

/// <summary>
/// Provides data management services for <see cref="StorageUnit"/> entities.
/// </summary>
public class StorageUnitService : IStorageUnitService
{
	private readonly IDbContextFactory<AppDbContext> _contextFactory;
	private readonly ILogger<StorageUnitService> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="StorageUnitService"/> class.
	/// </summary>
	/// <param name="contextFactory">The factory used to create <see cref="AppDbContext"/> instances.</param>
	/// <param name="logger">The logger for logging errors.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="contextFactory"/> is null.</exception>
	public StorageUnitService(IDbContextFactory<AppDbContext> contextFactory, ILogger<StorageUnitService> logger)
	{
		ArgumentNullException.ThrowIfNull(contextFactory);
		ArgumentNullException.ThrowIfNull(logger);

		_contextFactory = contextFactory;
		_logger = logger;
	}

	/// <inheritdoc />
	public async Task<IEnumerable<StorageUnit>> GetAll()
	{
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			return await context.StorageUnits.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving all storage units.");
			throw;
		}
	}

	/// <inheritdoc />
	/// <exception cref="KeyNotFoundException">Thrown when no storage unit with the specified ID is found.</exception>
	public async Task<StorageUnit> Get(int id)
	{
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			return await context.StorageUnits.FindAsync(id)
				   ?? throw new KeyNotFoundException($"StorageUnit with Id {id} not found");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving storage unit with Id {StorageUnitId}.", id);
			throw;
		}
	}

	/// <inheritdoc />
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
	/// <exception cref="UniqueConstraintException">Thrown when the entity violates domain validation or unique constraints.</exception>
	public async Task<StorageUnit> Add(StorageUnit item)
	{
		ArgumentNullException.ThrowIfNull(item);

		var validationErrors = await ValidateItem(item);

		if (validationErrors.Any())
		{
			throw new UniqueConstraintException("Invalid StorageUnit", validationErrors);
		}

		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			var addedEntity = context.StorageUnits.Add(item);

			await context.SaveChangesAsync();

			return addedEntity.Entity;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error adding storage unit.");
			throw;
		}
	}

	/// <inheritdoc />
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
	/// <exception cref="UniqueConstraintException">Thrown when the updated entity violates domain validation or unique constraints.</exception>
	/// <exception cref="KeyNotFoundException">Thrown when the storage unit does not exist in the database.</exception>
	public async Task<StorageUnit> Update(StorageUnit item)
	{
		ArgumentNullException.ThrowIfNull(item);

		var validationErrors = await ValidateItem(item);

		if (validationErrors.Any())
		{
			throw new UniqueConstraintException("Invalid StorageUnit", validationErrors);
		}

		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			var foundEntity = await context.StorageUnits.FindAsync(item.Id);

			if (foundEntity is not null)
			{
				context.Entry(foundEntity).CurrentValues.SetValues(item);
				await context.SaveChangesAsync();
			}

			return foundEntity ?? throw new KeyNotFoundException($"StorageUnit with Id {item.Id} not found");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating storage unit with Id {StorageUnitId}.", item.Id);
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

			var foundEntity = await context.StorageUnits.FirstOrDefaultAsync(p => p.Id == id);

			if (foundEntity == null)
			{
				return;
			}

			//Cascading delete will remove all linked shelves
			context.StorageUnits.Remove(foundEntity);
			await context.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting storage unit with Id {StorageUnitId}.", id);
			throw;
		}
	}

	/// <inheritdoc />
	public async Task<List<(string PropertyName, string ErrorMessage)>> ValidateItem(StorageUnit item)
	{
		try
		{
			var returnValue = new List<(string PropertyName, string ErrorMessage)>();

			await using var context = await _contextFactory.CreateDbContextAsync();

			//Unique index on name and room
			var duplicate = await context.StorageUnits
				.AnyAsync(s => s.Name == item.Name && s.RoomId == item.RoomId && s.Id != item.Id);

			if (duplicate)
			{
				returnValue.Add((nameof(item.Name), "A storage unit with this name already exists in this room."));
			}

			return returnValue;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error validating storage unit.");
			throw;
		}
	}
}
