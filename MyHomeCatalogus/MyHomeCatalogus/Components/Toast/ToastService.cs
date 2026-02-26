namespace MyHomeCatalogus.Components.Toast
{
    public enum ToastLevel { Info, Success, Warning, Error }

    public class ToastInstance
    {
        public string Message { get; set; } = string.Empty;
        public ToastLevel Level { get; set; }
        public bool IsFixed { get; set; }
    }

    public interface IToastService
    {
        event Action<string, ToastLevel, bool>? OnShow;
        void ShowToast(string message, ToastLevel level = ToastLevel.Info, bool isFixed = false);
        void NotifyToastClosed();
    }

    public class ToastService : IToastService
    {
        public event Action<string, ToastLevel, bool>? OnShow;

        private readonly Queue<ToastInstance> _queue = new();
        private const int MaxDisplayedToasts = 3;
        private int _activeCount = 0;

        public void ShowToast(string message, ToastLevel level = ToastLevel.Info, bool isFixed = false)
        {
            var instance = new ToastInstance { Message = message, Level = level, IsFixed = isFixed };
            _queue.Enqueue(instance);
            ProcessQueue();
        }

        private void ProcessQueue()
        {
            if (_activeCount < MaxDisplayedToasts && _queue.TryDequeue(out var next))
            {
                _activeCount++;
                OnShow?.Invoke(next.Message, next.Level, next.IsFixed);
            }
        }

        public void NotifyToastClosed()
        {
            _activeCount--;
            ProcessQueue();
        }
    }
}
