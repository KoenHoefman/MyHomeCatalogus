using MyHomeCatalogus.Components.Widgets;

namespace MyHomeCatalogus.Components.Pages
{
    public partial class Home
    {
        public List<Type> Widgets { get; set; } = new List<Type>() { typeof(ShoppingListWidget), typeof(ProductThresholdWidget) };
    }
}
