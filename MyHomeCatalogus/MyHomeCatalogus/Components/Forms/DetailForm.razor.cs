using Microsoft.AspNetCore.Components;

namespace MyHomeCatalogus.Components.Forms
{
	public partial class DetailForm
	{
		[Parameter, EditorRequired]
		public required RenderFragment ChildContent { get; set; }

		[Parameter, EditorRequired]
		public required string EditRoute { get; set; }

		[Parameter, EditorRequired]
		public required string DeleteRoute { get; set; }

		[Parameter, EditorRequired]
		public required string BackToListRoute { get; set; }

	}
}
