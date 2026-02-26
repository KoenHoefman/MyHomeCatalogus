using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Extensions
{
    public static class ShelfExtensions
    {
        public static string ToDisplayString(this Shelf shelf)
        {
            return shelf.StorageUnit is null ? shelf.Name : $"{shelf.Name} [Storage unit: {shelf.StorageUnit.ToDisplayString()}]";
        }
    }
}
