using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Repository;

namespace DoAnTotNghiep.Services
{
    public class NotificationService : INotificationServices
    {
        private readonly INotificationRepository _notificationServices;
        public NotificationService(INotificationRepository notificationServices)
        {
            _notificationServices = notificationServices;
        }

        public async Task<int> AddNotification(NotificationToAddDTO notificationToAdd)
        {
            return await _notificationServices.AddNotification(notificationToAdd);
        }

        public async Task<List<NotificationToShowDTO>> GetNotification(string UserName)
        {
            return await _notificationServices.GetNotification(UserName);
        }
    }
}
