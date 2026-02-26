using Microsoft.AspNetCore.Components;

namespace MyHomeCatalogus.Components.Forms
{
    public partial class DetailDeleteFormRow
    {
        [Parameter, EditorRequired]
        public RenderFragment LabelContent { get; set; } = default!;

        [Parameter, EditorRequired]
        public RenderFragment ValueContent { get; set; } = default!;

    }
}
