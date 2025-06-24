using DoAnTotNghiep.DTOs;

namespace DoAnTotNghiep.Services
{
    public interface INotificationServices
    {
        Task<List<NotificationToShowDTO>> GetNotification(string UserName);
        Task<int> AddNotification(NotificationToAddDTO notificationToAdd);
    }
}
