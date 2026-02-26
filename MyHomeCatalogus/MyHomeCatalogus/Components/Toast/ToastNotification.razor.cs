using Microsoft.AspNetCore.Components;

namespace MyHomeCatalogus.Components.Toast
{
    public partial class ToastNotification : IDisposable
    {
        private class ActiveToast
        {
            public required ToastInstance Instance { get; set; }
            public CancellationTokenSource? Cts { get; set; }
        }


        [Inject]
        public required IToastService ToastService { get; set; }

        private List<ActiveToast> _activeToasts = new();


        protected override void OnInitialized()
        {
            ToastService.OnShow += HandleShow;
        }

        private void HandleShow(string message, ToastLevel level, bool isFixed)
        {
            var activeToast = new ActiveToast
            {
                Instance = new ToastInstance { Message = message, Level = level, IsFixed = isFixed }
            };

            _activeToasts.Add(activeToast);
            InvokeAsync(StateHasChanged);

            if (!isFixed) StartCountdown(activeToast);
        }

        private void StartCountdown(ActiveToast toast)
        {
            toast.Cts = new CancellationTokenSource();
            var token = toast.Cts.Token;

            Task.Delay(5000, token).ContinueWith(t =>
            {
                if (!t.IsCanceled)
                {
                    RemoveToast(toast);
                }
            }, CancellationToken.None);
        }

        private void RemoveToast(ActiveToast toast)
        {
            _activeToasts.Remove(toast);
            ToastService.NotifyToastClosed();
            InvokeAsync(StateHasChanged);
        }

        private void OnMouseEnter(ActiveToast toast)
        {
            toast.Cts?.Cancel();
        }

        private void OnMouseLeave(ActiveToast toast)
        {
            if (!toast.Instance.IsFixed)
            {
                StartCountdown(toast);
            }
        }

        private string GetHeaderClass(ToastLevel level) => level switch
        {
            ToastLevel.Success => "bg-success",
            ToastLevel.Error => "bg-danger",
            ToastLevel.Warning => "bg-warning text-dark",
            _ => "bg-primary"
        };

        private string GetIcon(ToastLevel level) => level switch
        {
            ToastLevel.Success => "bi bi-check-circle-fill",
            ToastLevel.Error => "bi bi-exclamation-octagon-fill",
            ToastLevel.Warning => "bi bi-exclamation-triangle-fill",
            _ => "bi bi-info-circle-fill"
        };

        public void Dispose() => ToastService.OnShow -= HandleShow;
    }
}
