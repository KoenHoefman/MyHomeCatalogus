using MyHomeCatalogus.Shared.Domain;
using System.Linq.Expressions;

namespace MyHomeCatalogus.Interfaces;

/// <summary>
/// Defines specialized data operations for <see cref="Product"/> entities, extending the base data service capabilities.
/// </summary>
public interface IProductService : IDataService<Product>
{
	/// <summary>
	/// Retrieves a collection of products based on an optional filter expression.
	/// </summary>
	/// <param name="filter">An optional LINQ expression to filter the products (e.g., filtering by name or <see cref="ProductType"/>).</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains an enumeration of filtered products.</returns>
	Task<IEnumerable<Product>> GetAll(Expression<Func<Product, bool>>? filter);
}