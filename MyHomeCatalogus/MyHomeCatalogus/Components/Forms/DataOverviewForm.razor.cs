using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Interfaces;

namespace MyHomeCatalogus.Components.Forms
{
    public partial class DataOverviewForm<TItem> where TItem : class, IEntity
    {
        [Parameter, EditorRequired]
        public required IDataService<TItem> DataService { get; set; }

        [Parameter, EditorRequired]
        public required string Title { get; set; }

        [Parameter, EditorRequired]
        public required string BaseRoute { get; set; }

        [Parameter]
        public RenderFragment<TItem>? GridColumns { get; set; }

        [Parameter]
        public bool ShowView { get; set; } = true;

        [Parameter]
        public bool ShowEdit { get; set; } = true;

        [Parameter]
        public bool ShowDelete { get; set; } = true;

        [Parameter]
        public RenderFragment<TItem>? CustomActions { get; set; }

        [Parameter]
        public string AddItemText { get; set; } = "Add new";

        [Parameter]
        public RenderFragment? FilterTemplate { get; set; }

        private IFilterProvider<TItem>? _activeFilter;
        private bool HasActiveFilters => _activeFilter?.IsActive ?? false;

        private RenderFragment? EffectiveGridColumns { get; set; }
        private IQueryable<TItem>? _items;
        private string? _errorMessage;
        private bool _isLoading;

        private IQueryable<TItem>? FilteredItems
        {
            get
            {
                if (_items is null)
                {
                    return null;
                }

                return _items.AsEnumerable()
                    .Where(item => _activeFilter?.Matches(item) ?? true)
                    .AsQueryable();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                _isLoading = true;
                _errorMessage = null;

                var data = await DataService.GetAll();
                _items = data.AsQueryable();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _errorMessage = $"Unable to load data: {ex.Message}";
            }
            finally
            {
                _isLoading = false;
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (GridColumns != null)
            {
                // Assign the custom fragment directly.
                // RenderFragment<TItem> is implicitly convertible to RenderFragment here.
                EffectiveGridColumns = GridColumns.Invoke(default!);
            }
            else
            {
                // Fallback (Configuration Error)
                // If no configuration is provided, show the error column using RenderTreeBuilder manually.
                EffectiveGridColumns = builder =>
                {
                    builder.OpenComponent<TemplateColumn<TItem>>(0);
                    builder.AddComponentParameter(1, "Title", "Configuration Error");
                    builder.AddComponentParameter(2, "ChildContent", (RenderFragment<TItem>)((item) => (innerBuilder) =>
                    {
                        innerBuilder.AddMarkupContent(3, "<p class=\"text-red-500 font-bold p-2\">ERROR: You must define GridColumns to use this component.</p>");
                    }));
                    builder.CloseComponent();
                };
            }
        }

        public async Task RefreshAsync()
        {
            await LoadDataAsync();
            StateHasChanged();
        }

        public void RegisterFilter(IFilterProvider<TItem> filter)
        {
            _activeFilter = filter;
            StateHasChanged();
        }

        private async Task ClearFiltersAsync()
        {
            if (_activeFilter != null)
            {
                _activeFilter.Reset();
                await RefreshAsync();
            }
        }
    }
}
