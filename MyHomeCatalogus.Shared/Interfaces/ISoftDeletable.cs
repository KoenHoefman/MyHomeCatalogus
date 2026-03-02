namespace MyHomeCatalogus.Shared.Interfaces
{
	public interface ISoftDeletable
	{
		bool IsDeleted { get; set; }
		DateTime DateDeleted { get; set; }
	}
}
