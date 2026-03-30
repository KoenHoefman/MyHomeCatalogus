using MyHomeCatalogus.Data;

namespace MyHomeCatalogus.Interfaces
{
    public interface IUserNotificationService
    {
        Task NotifyAdminsNewUserConfirmedAsync(ApplicationUser user);
    }
}
