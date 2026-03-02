using Microsoft.AspNetCore.Components;

namespace MyHomeCatalogus.Components.Forms
{
	public partial class DeleteForm<TItem>
	{
		[Parameter, EditorRequired]
		public required TItem Item { get; set; }

		[Parameter, EditorRequired]
		public required EventCallback OnValidSubmit { get; set; }

		[Parameter, EditorRequired]
		public required RenderFragment ChildContent { get; set; }

		[Parameter, EditorRequired]
		public required string BackToListRoute { get; set; }

		[Parameter, EditorRequired]
		public bool IsProcessing { get; set; }

		[Parameter]
		public string FormName { get; set; } = "deleteform";

		[Parameter]
		public RenderFragment? DeleteMessage { get; set; }

	}
}
