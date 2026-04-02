using MyHomeCatalogus.Components.Toast;
using Xunit;

namespace MyHomeCatalogus.UnitTests.Components.Toast
{
    public class ToastServiceTests
    {
        [Fact]
        public void ShowToast_Should_InvokeEvent_When_UnderMaxDisplayed()
        {
            var service = new ToastService();
            var invokedMessages = new List<(string message, ToastLevel level, bool isFixed)>();

            service.OnShow += (msg, level, isFixed) => invokedMessages.Add((msg, level, isFixed));

            service.ShowToast("Test Message", ToastLevel.Success, true);

            Assert.Single(invokedMessages);
            Assert.Equal("Test Message", invokedMessages[0].message);
            Assert.Equal(ToastLevel.Success, invokedMessages[0].level);
            Assert.True(invokedMessages[0].isFixed);
        }

        [Fact]
        public void ShowToast_Should_Queue_When_AtMaxDisplayed()
        {
            var service = new ToastService();
            var invokedMessages = new List<string>();

            service.OnShow += (msg, level, isFixed) => invokedMessages.Add(msg);

            // Show 3 toasts, all should invoke
            for (int i = 1; i <= 3; i++)
            {
                service.ShowToast($"Message {i}");
            }

            Assert.Equal(3, invokedMessages.Count);

            // 4th should not invoke yet
            service.ShowToast("Message 4");
            Assert.Equal(3, invokedMessages.Count);

            // Close one, should process next
            service.NotifyToastClosed();
            Assert.Equal(4, invokedMessages.Count);
            Assert.Equal("Message 4", invokedMessages[3]);
        }

        [Fact]
        public void ShowToast_Should_UseDefaultValues()
        {
            var service = new ToastService();
            var invokedMessages = new List<(string message, ToastLevel level, bool isFixed)>();

            service.OnShow += (msg, level, isFixed) => invokedMessages.Add((msg, level, isFixed));

            service.ShowToast("Default Test");

            Assert.Single(invokedMessages);
            Assert.Equal("Default Test", invokedMessages[0].message);
            Assert.Equal(ToastLevel.Info, invokedMessages[0].level);
            Assert.False(invokedMessages[0].isFixed);
        }

        [Fact]
        public void NotifyToastClosed_Should_ProcessQueue()
        {
            var service = new ToastService();
            var invokedMessages = new List<string>();

            service.OnShow += (msg, level, isFixed) => invokedMessages.Add(msg);

            // Fill queue
            for (int i = 1; i <= 4; i++)
            {
                service.ShowToast($"Message {i}");
            }

            Assert.Equal(3, invokedMessages.Count); // Only 3 displayed

            // Close one
            service.NotifyToastClosed();
            Assert.Equal(4, invokedMessages.Count); // Now 4

            // Close another
            service.NotifyToastClosed();
            Assert.Equal(4, invokedMessages.Count); // No more in queue
        }

        [Fact]
        public void NotifyToastClosed_Should_HandleMultipleCloses()
        {
            var service = new ToastService();
            var invokedMessages = new List<string>();

            service.OnShow += (msg, level, isFixed) => invokedMessages.Add(msg);

            service.ShowToast("Message 1");
            service.ShowToast("Message 2");

            Assert.Equal(2, invokedMessages.Count);

            // Close more than active
            service.NotifyToastClosed();
            service.NotifyToastClosed();
            service.NotifyToastClosed(); // Should not crash

            Assert.Equal(2, invokedMessages.Count);
        }
    }
}