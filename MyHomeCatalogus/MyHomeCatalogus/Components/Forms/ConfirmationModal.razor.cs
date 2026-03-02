using Microsoft.AspNetCore.Components;

namespace MyHomeCatalogus.Components.Forms
{
	public partial class ConfirmationModal<TItem>
	{
		[Parameter]
		public string Message { get; set; } = "Are you sure?";

		[Parameter]
		public string AcceptButtonText { get; set; } = "Yes";

		[Parameter]
		public string CancelButtonText { get; set; } = "No";

		/// <summary>
		/// Shows the text "This action cannot be undone." below the message when <code>true</code>.
		/// Default is <code>false</code>.
		/// </summary>
		[Parameter]
		public bool ActionCanBeReversed { get; set; } = false;

		[Parameter]
		public EventCallback<TItem> OnConfirmed { get; set; }

		private bool _isVisible;
		private TItem? _itemToProcess;

		public void Show(TItem item)
		{
			_itemToProcess = item;
			_isVisible = true;
			StateHasChanged();
		}

		public void Close() => _isVisible = false;

		private async Task Confirm()
		{
			if (_itemToProcess != null)
			{
				await OnConfirmed.InvokeAsync(_itemToProcess);
			}
			Close();
		}
	}
}
