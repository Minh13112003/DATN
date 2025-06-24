using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationServices _notificationServices;
        public NotificationController(INotificationServices notificationServices)
        {
            _notificationServices = notificationServices;
        }
        [Authorize]
        [HttpGet("getNotification")]
        public async Task<IActionResult> GetNotification()
        {
            var user = User.Identity?.Name;
            var notification = await _notificationServices.GetNotification(user);
            if (notification == null)
            {
                return Ok(new { message = "Bạn chưa có thông báo nào" });
            }
            return Ok(notification);
        }
        
    }
}
