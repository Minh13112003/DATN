using DoAnTotNghiep.Data;
using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Helper.DateTimeVietNam;
using DoAnTotNghiep.Model;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DoAnTotNghiep.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DatabaseContext _databaseContext;
        public NotificationRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<int> AddNotification(NotificationToAddDTO notificationToAdd)
        {
            if(notificationToAdd.UserName == null) return -2;
            if(notificationToAdd.Content == null) return -1;
            Notification notification = new Notification
            {
                IdNotice = new Guid().ToString(),
                Content = notificationToAdd.Content,
                UserName = notificationToAdd.UserName,
                Idcomment = notificationToAdd.Idcomment,
                TitleMovie = notificationToAdd.TitleMovie
            };
            await _databaseContext.Notification.AddAsync(notification);
            await _databaseContext.SaveChangesAsync();
            return 1;
        }

        public async Task<List<NotificationToShowDTO>> GetNotification(string UserName)
        {
            var sql = @"
                SELECT 
                    nt.""IdNotice"",
                    nt.""IdComment"",
                    nt.""UserName"",
                    nt.""Content"",
                    nt.""CreatedAt"",
                    nt.""TitleMovie""
                FROM ""Notification"" nt
                WHERE nt.""UserName"" = @username";
            return await _databaseContext.Database
        .SqlQueryRaw<NotificationToShowDTO>(sql, new[] { new NpgsqlParameter("@username", UserName) })
            .ToListAsync();
        }
    }
}
