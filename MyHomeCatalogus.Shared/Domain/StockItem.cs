using MyHomeCatalogus.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MyHomeCatalogus.Shared.Domain;

public class StockItem : IEntity
{
	public int Id { get; set; }

	[Range(1, int.MaxValue, ErrorMessage = "Product is required.")]
	public int ProductId { get; set; }

	public Product? Product { get; set; }

	[Range(1, int.MaxValue, ErrorMessage = "Location is required.")]
	public int ShelfId { get; set; }

	public Shelf? Shelf { get; set; }

	[Range(1, int.MaxValue, ErrorMessage = "StockUnit is required.")]
	public int StockUnitId { get; set; }

	public StockUnit? StockUnit { get; set; }

	[Range(0, int.MaxValue, ErrorMessage = "Quantity must be 0 or greater.")]
	public int Quantity { get; set; }

}