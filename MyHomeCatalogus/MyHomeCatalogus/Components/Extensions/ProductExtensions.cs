using MyHomeCatalogus.Shared.Domain;
using System;

namespace MyHomeCatalogus.Components.Extensions
{
    public static class ProductExtensions
    {
        public static string ToDisplayString(this Product product)
        {
            return product.Store is null ? product.Name : $"{product.Name} ({product.Store.Name})";
        }
    }
}
