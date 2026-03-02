using Microsoft.AspNetCore.Components;

namespace MyHomeCatalogus.Components.Forms
{
	public partial class DetailDeleteFormRow
	{
		[Parameter, EditorRequired]
		public RenderFragment LabelContent { get; set; } = null!;

		[Parameter, EditorRequired]
		public RenderFragment ValueContent { get; set; } = null!;

	}
}
