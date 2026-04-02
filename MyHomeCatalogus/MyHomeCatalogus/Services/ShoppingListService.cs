using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;

namespace MyHomeCatalogus.Services;

/// <summary>
/// Provides data management services for <see cref="ShoppingList"/> entities.
/// </summary>
public class ShoppingListService : IShoppingListService
{
	private readonly IDbContextFactory<AppDbContext> _contextFactory;
	private readonly ILogger<ShoppingListService> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="ShoppingListService"/> class.
	/// </summary>
	/// <param name="contextFactory">The factory used to create <see cref="AppDbContext"/> instances.</param>
	/// <param name="logger">The logger for logging errors.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="contextFactory"/> is null.</exception>
	public ShoppingListService(IDbContextFactory<AppDbContext> contextFactory, ILogger<ShoppingListService> logger)
	{
		ArgumentNullException.ThrowIfNull(contextFactory);
		ArgumentNullException.ThrowIfNull(logger);

		_contextFactory = contextFactory;
		_logger = logger;
	}

	/// <inheritdoc />
	public async Task<IEnumerable<ShoppingList>> GetAll()
	{
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			return await context.ShoppingLists.Include(p => p.Store).ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving all shopping lists.");
			throw;
		}
	}


	/// <inheritdoc />
	/// <exception cref="KeyNotFoundException">Thrown when no shopping list with the specified ID is found.</exception>
	public async Task<ShoppingList> Get(int id)
	{
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			return await context.ShoppingLists
					   .Include(p => p.Store)
					   .Include(p => p.ShoppingListItems)
					   .ThenInclude(i => i.Product)
					   .SingleOrDefaultAsync(p => p.Id == id)
				   ?? throw new KeyNotFoundException($"ShoppingList with Id {id} not found");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving shopping list with Id {ShoppingListId}.", id);
			throw;
		}
	}


	/// <inheritdoc />
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
	/// <exception cref="UniqueConstraintException">Thrown when the entity violates domain validation or unique constraints.</exception>
	public async Task<ShoppingList> Add(ShoppingList item)
	{
		ArgumentNullException.ThrowIfNull(item);

		var validationErrors = await ValidateItem(item);

		if (validationErrors.Any())
		{
			throw new UniqueConstraintException("Invalid ShoppingList", validationErrors);
		}

		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			var addedEntity = context.ShoppingLists.Add(item);

			await context.SaveChangesAsync();

			return addedEntity.Entity;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error adding shopping list.");
			throw;
		}
	}

	/// <inheritdoc />
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
	/// <exception cref="UniqueConstraintException">Thrown when the updated entity violates domain validation or unique constraints.</exception>
	/// <exception cref="KeyNotFoundException">Thrown when the shopping list does not exist.</exception>
	/// <exception cref="InvalidOperationException">Thrown when a business rule prevents the update operation.</exception>
	public async Task<ShoppingList> Update(ShoppingList item)
	{
		ArgumentNullException.ThrowIfNull(item);

		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			//Store can't be changed when there are items in the list
			var existingList = await context.ShoppingLists
				.Include(s => s.ShoppingListItems)
				.FirstOrDefaultAsync(s => s.Id == item.Id);

			if (existingList == null)
			{
				throw new KeyNotFoundException($"Shopping list with ID {item.Id} not found.");
			}

			if (existingList.ShoppingListItems.Any() && existingList.StoreId != item.StoreId)
			{
				throw new InvalidOperationException("Cannot change the store because the shopping list already contains items.");
			}

			var validationErrors = await ValidateItem(item);

			if (validationErrors.Any())
			{
				throw new UniqueConstraintException("Invalid ShoppingList", validationErrors);
			}

			var foundEntity = await context.ShoppingLists.FindAsync(item.Id);

			if (foundEntity is not null)
			{
				context.Entry(foundEntity).CurrentValues.SetValues(item);
				await context.SaveChangesAsync();
			}

			return foundEntity ?? throw new KeyNotFoundException($"ShoppingList with Id {item.Id} not found");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating shopping list with Id {ShoppingListId}.", item.Id);
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

			var foundEntity = await context.ShoppingLists.FirstOrDefaultAsync(p => p.Id == id);

			if (foundEntity == null)
			{
				return;
			}

			context.ShoppingLists.Remove(foundEntity);
			await context.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting shopping list with Id {ShoppingListId}.", id);
			throw;
		}
	}

	/// <inheritdoc />
	public async Task<IEnumerable<ShoppingList>> GetAllActiveShoppingLists()
	{
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			return await context.ShoppingLists
				.Where(p => !p.IsCompleted)
				.Include(p => p.Store)
				.ToListAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving active shopping lists.");
			throw;
		}
	}

	/// <inheritdoc />
	public async Task<IEnumerable<ShoppingListWidgetData>> GetItemsCountForActiveShoppingLists()
	{
		try
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			var activeLists = await context.ShoppingLists
				.Where(p => !p.IsCompleted)
				.Include(p => p.Store)
				.Include(p => p.ShoppingListItems)
				.AsNoTracking()
				.ToListAsync();

			var returnValue = new List<ShoppingListWidgetData>();

			foreach (var shoppingList in activeLists)
			{
				returnValue.Add(new ShoppingListWidgetData(shoppingList));
			}

			return returnValue;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error building active shopping list widget data.");
			throw;
		}
	}


	/// <inheritdoc />
	public async Task<List<(string PropertyName, string ErrorMessage)>> ValidateItem(ShoppingList item)
	{
		try
		{
			var returnValue = new List<(string PropertyName, string ErrorMessage)>();

			await using var context = await _contextFactory.CreateDbContextAsync();

			//Only 1 active list per store
			if (!item.IsCompleted)
			{
				var duplicate = await context.ShoppingLists
					.AnyAsync(s => s.StoreId == item.StoreId && !s.IsCompleted && s.Id != item.Id);

				if (duplicate)
				{
					returnValue.Add((nameof(item.StoreId), "There is already an active shoppinglist for this store."));
				}
			}

			return returnValue;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error validating shopping list.");
			throw;
		}
	}
}
