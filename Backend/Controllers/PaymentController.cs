using DoAnTotNghiep.Model;
using DoAnTotNghiep.Model.VnPayModel;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using Net.payOS.Types;

namespace DoAnTotNghiep.Controllers
{

    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IPayOsService _payOsService;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(IVnPayService vnPayService, UserManager<AppUser> userManager, IPayOsService payOsService, ILogger<PaymentController> logger)
        {
            _payOsService = payOsService;
            _vnPayService = vnPayService;
            _userManager = userManager;
            _logger = logger;
        }
        [Authorize]
        [HttpPost("create-payment")]
        public IActionResult CreatePaymentUrlVnpay([FromBody] PaymentRequestModel model)
        {
            var UserName = User.Identity?.Name;
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext, UserName!);

            return Ok(url);
        }
        [HttpGet("payment-return")]
        public async Task<IActionResult> PaymentReturn()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response.VnPayResponseCode == "00" && response.Success)
            {
                var orderInfo = Request.Query["vnp_OrderInfo"].ToString();
                var parts = orderInfo.Split(' ');
                var userName = parts.Length > 0 ? parts[0] : ""; // bạn cần viết hàm này

                // Cập nhật IsVip = true
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    user.IsVip = true;
                    await _userManager.UpdateAsync(user);
                }
            }
            return Ok(response);
        }
        [Authorize]
        [HttpPost("Payos/create-payment")]
        public async Task<IActionResult> CreatePaymentUrl([FromQuery] int type)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var UserName = User.Identity?.Name;
                var Urlstring = await _payOsService.CreatePaymentLink(type, UserName!);
                if (Urlstring != null) { return Ok(Urlstring); }
                return BadRequest("Đã có lỗi");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("Payos/PayOsWebhook")]
        public async Task<IActionResult> ReceiveWebhook()
        {
            try
            {
                using var reader = new StreamReader(Request.Body, Encoding.UTF8);
                string body = await reader.ReadToEndAsync();
                _logger.LogInformation("Webhook body: {Body}", body);

                // Log toàn bộ headers để kiểm tra
                var webhookData = JsonSerializer.Deserialize<WebhookType>(body);

                if (webhookData == null || string.IsNullOrEmpty(webhookData.signature))
                {
                    _logger.LogWarning("Webhook thiếu chữ ký.");
                    return Ok();
                }

                bool result = await _payOsService.HandleWebhook(body, webhookData.signature!);

                if (!result)
                {
                    _logger.LogWarning("Webhook xử lý thất bại hoặc sai chữ ký.");
                }

                return Ok(); // ✅ Dù xử lý thành công hay không, vẫn trả về 200 OK
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý webhook PayOS");
                return Ok(); // ✅ Ngay cả khi lỗi, vẫn trả 200 OK để tránh retry loop
            }
        }
        [Authorize]
        [HttpGet("Payos/GetPaymentOrder")]
        public async Task<IActionResult> GetPaymentOrder()
        {
            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(new { message = "Không xác định được người dùng" });
            }

            var payments = await _payOsService.GetPaymentOrders(userName);

            if (payments == null || !payments.Any())
            {
                return Ok(new
                {
                    message = "Bạn chưa thanh toán đơn nào",
                    data = new List<PaymentOrder>()
                });
            }

            return Ok(new
            {
                message = "Lấy thông tin thanh toán thành công",
                data = payments
            });
        }
        [Authorize]
        [HttpGet("Payos/GetDetailPayment/{OrderCode?}")]
        public async Task<IActionResult> GetDetailPayment(long? OrderCode)
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(new { message = "Không tìm thấy người dùng" });
            }
            if (OrderCode == null)
            {
                return BadRequest(new { message = "Mã đơn hàng bị thiếu" });
            }
            var detailPayment = await _payOsService.GetDetailPaymentOrders(userName, OrderCode);
            if (detailPayment == null) return BadRequest(new { message = "Không tìm thấy đơn hàng" });
            return Ok(detailPayment);
        }
        [Authorize]
        [HttpPost("Payos/CancelPayment/{ordercode}")]
        public async Task<IActionResult> CancelPayment([FromRoute] long ordercode)
        {
            var cancelpayment = await _payOsService.CancelPaymentOrder(ordercode);
            if (cancelpayment == true) return Ok(new { message = "Hủy đơn thành công" });
            return BadRequest(new { message = "Đã có lỗi xảy ra" });
        }
    }
}
