using DoAnTotNghiep.DTOs;

namespace DoAnTotNghiep.Repository
{
    public interface INotificationRepository
    {
        Task<List<NotificationToShowDTO>> GetNotification(string UserName);
        Task<int> AddNotification(NotificationToAddDTO notificationToAdd);
    }
}
