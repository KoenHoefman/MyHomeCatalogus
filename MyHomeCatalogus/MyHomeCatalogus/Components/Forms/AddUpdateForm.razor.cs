using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Components.Extensions;

namespace MyHomeCatalogus.Components.Forms
{
	public partial class AddUpdateForm<TItem>
	{
		[Parameter, EditorRequired]
		public required EditContext EditContext { get; set; }

		[Parameter, EditorRequired]
		public required EventCallback OnValidSubmit { get; set; }

		[Parameter, EditorRequired]
		public required RenderFragment ChildContent { get; set; }

		[Parameter, EditorRequired]
		public required string BackToListRoute { get; set; }

		[Parameter, EditorRequired]
		public bool IsProcessing { get; set; }

		[Parameter]
		public string FormName { get; set; } = "addupdateform";

		private ValidationMessageStore _messageStore = null!;

		protected override void OnInitialized()
		{
			_messageStore = new ValidationMessageStore(EditContext);
			EditContext.Properties["ValidationStore"] = _messageStore;
		}

		private async Task HandleSubmitInternal()
		{
			if (IsProcessing)
			{
				return;
			}

			EditContext.ClearValidationErrors();

			if (EditContext.Validate())
			{
				await OnValidSubmit.InvokeAsync();
			}
		}
	}
}
