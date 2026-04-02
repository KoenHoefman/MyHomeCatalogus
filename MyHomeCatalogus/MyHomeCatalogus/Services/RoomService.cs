using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Services;

/// <summary>
/// Provides data management services for <see cref="Room"/> entities.
/// </summary>
public class RoomService : IRoomService
{
	private readonly IDbContextFactory<AppDbContext> _contextFactory;
	private readonly ILogger<RoomService> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="RoomService"/> class.
	/// </summary>
	/// <param name="contextFactory">The factory used to create <see cref="AppDbContext"/> instances.</param>
	/// <param name="logger">The logger for logging errors.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="contextFactory"/> is null.</exception>
	public RoomService(IDbContextFactory<AppDbContext> contextFactory, ILogger<RoomService> logger)
	{
		ArgumentNullException.ThrowIfNull(contextFactory);
		ArgumentNullException.ThrowIfNull(logger);

		_contextFactory = contextFactory;
		_logger = logger;
	}

	/// <inheritdoc />
	public async Task<IEnumerable<Room>> GetAll()
	{
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			return await context.Rooms.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving all rooms.");
			throw;
		}
	}

	/// <inheritdoc />
	/// <exception cref="KeyNotFoundException">Thrown when no room with the specified ID is found.</exception>
	public async Task<Room> Get(int id)
	{
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			return await context.Rooms.FindAsync(id)
				   ?? throw new KeyNotFoundException($"Room with Id {id} not found");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving room with Id {RoomId}.", id);
			throw;
		}
	}

	/// <inheritdoc />
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
	/// <exception cref="UniqueConstraintException">Thrown when the entity violates domain validation or unique constraints.</exception>
	public async Task<Room> Add(Room item)
	{
		ArgumentNullException.ThrowIfNull(item);

		var validationErrors = await ValidateItem(item);

		if (validationErrors.Any())
		{
			throw new UniqueConstraintException("Invalid Room", validationErrors);
		}

		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			var addedEntity = context.Rooms.Add(item);

			await context.SaveChangesAsync();

			return addedEntity.Entity;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error adding room.");
			throw;
		}
	}

	/// <inheritdoc />
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
	/// <exception cref="UniqueConstraintException">Thrown when the updated entity violates domain validation or unique constraints.</exception>
	/// <exception cref="KeyNotFoundException">Thrown when the room does not exist in the database.</exception>
	public async Task<Room> Update(Room item)
	{
		ArgumentNullException.ThrowIfNull(item);

		var validationErrors = await ValidateItem(item);

		if (validationErrors.Any())
		{
			throw new UniqueConstraintException("Invalid Room", validationErrors);
		}

		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			var foundEntity = await context.Rooms.FindAsync(item.Id);

			if (foundEntity is not null)
			{
				context.Entry(foundEntity).CurrentValues.SetValues(item);
				await context.SaveChangesAsync();
			}

			return foundEntity ?? throw new KeyNotFoundException($"Room with Id {item.Id} not found");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating room with Id {RoomId}.", item.Id);
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

			var foundEntity = await context.Rooms.FirstOrDefaultAsync(p => p.Id == id);

			if (foundEntity == null)
			{
				return;
			}

			//Cascading delete will remove all linked storage units
			context.Rooms.Remove(foundEntity);
			await context.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting room with Id {RoomId}.", id);
			throw;
		}
	}

	/// <inheritdoc />
	public async Task<List<(string PropertyName, string ErrorMessage)>> ValidateItem(Room item)
	{
		try
		{
			var returnValue = new List<(string PropertyName, string ErrorMessage)>();

			await using var context = await _contextFactory.CreateDbContextAsync();

			//Unique index on Name
			var duplicate = await context.Rooms
				.AnyAsync(s => s.Name == item.Name && s.Id != item.Id);

			if (duplicate)
			{
				returnValue.Add((nameof(item.Name), "A room with this name already exists."));
			}

			return returnValue;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error validating room.");
			throw;
		}
	}
}
