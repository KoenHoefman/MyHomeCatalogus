using MyHomeCatalogus.Shared.Interfaces;

namespace MyHomeCatalogus.Interfaces;

/// <summary>
/// Defines the standard CRUD operations and validation logic for entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of entity, which must implement <see cref="IEntity"/>.</typeparam>
public interface IDataService<T> where T : class, IEntity
{
	/// <summary>
	/// Retrieves all entities of type <typeparamref name="T"/> from the data store.
	/// </summary>
	/// <returns>A collection of all entities.</returns>
	Task<IEnumerable<T>> GetAll();

	/// <summary>
	/// Retrieves a specific entity by its unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the entity.</param>
	/// <returns>The found entity.</returns>
	/// <exception cref="KeyNotFoundException">Thrown when no entity with the provided <paramref name="id"/> exists.</exception>
	Task<T> Get(int id);

	/// <summary>
	/// Adds a new entity to the data store.
	/// </summary>
	/// <param name="item">The entity to persist.</param>
	/// <returns>The persisted entity, including any database-generated values.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
	Task<T> Add(T item);

	/// <summary>
	/// Updates an existing entity in the data store.
	/// </summary>
	/// <param name="item">The entity containing updated values.</param>
	/// <returns>The updated entity.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
	/// <exception cref="KeyNotFoundException">Thrown when the entity to update does not exist in the data store.</exception>
	Task<T> Update(T item);

	/// <summary>
	/// Deletes an entity from the data store by its unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the entity to remove.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task Delete(int id);

	/// <summary>
	/// Validates an entity against domain-specific business rules.
	/// </summary>
	/// <param name="item">The entity to validate.</param>
	/// <returns>A list of tuples containing the property name and the associated error message.</returns>
	Task<List<(string PropertyName, string ErrorMessage)>> ValidateItem(T item);
}