using Microsoft.AspNetCore.Components;

namespace MyHomeCatalogus.Components.Forms
{
	public partial class AddUpdateFormRow
	{
		[Parameter, EditorRequired]
		public RenderFragment LabelContent { get; set; } = null!;

		[Parameter, EditorRequired]
		public RenderFragment InputContent { get; set; } = null!;

		[Parameter]
		public string? CustomClass { get; set; }

	}
}
