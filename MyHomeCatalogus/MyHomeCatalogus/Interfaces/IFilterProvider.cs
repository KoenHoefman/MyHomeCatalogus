using MyHomeCatalogus.Components.Forms;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Interfaces;

namespace MyHomeCatalogus.Interfaces
{
    public interface IFilterProvider<TItem> where TItem : class, IEntity
    {
        bool IsActive { get; }

        /// <summary>
        /// Determines if the item should be visible based on current filter state.
        /// </summary>
        bool Matches(TItem item);

       /// <summary>
       /// Resets the filter
       /// </summary>
        void Reset();
    }
}
