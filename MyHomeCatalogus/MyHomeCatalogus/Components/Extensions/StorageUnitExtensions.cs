using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Components.Extensions
{
    public static class StorageUnitExtensions
    {
        public static string ToDisplayString(this StorageUnit storageUnit)
        {
            return storageUnit.Room is null ? storageUnit.Name : $"{storageUnit.Name} [Room: {storageUnit.Room.Name}]";
        }
    }
}
