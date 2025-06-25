
using DoAnTotNghiep.Data;
using DoAnTotNghiep.Helper.DateTimeVietNam;
using DoAnTotNghiep.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace DoAnTotNghiep.Services
{
    public class PayOsService : IPayOsService
    {
        private readonly IConfiguration _configuration;
        private readonly PayOS _payos;
        private readonly ILogger<PayOsService> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly DatabaseContext _databaseContext;

        public PayOsService(IConfiguration configuration, ILogger<PayOsService> logger, DatabaseContext databaseContext, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _payos = new PayOS(_configuration["PayOS:Client_id"]!, _configuration["PayOS:API_Key"]!, _configuration["PayOS:Checksum_Key"]!);
            _userManager = userManager;
            _databaseContext = databaseContext;
            _logger = logger;
        }
        public static long GenerateOrderCode()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // 13 chữ số
            int randomPart = new Random().Next(100, 999); // 3 chữ số
            string codeStr = (timestamp % 1_000_000_000).ToString() + randomPart.ToString();
            return long.Parse(codeStr); 
        }
        private static string ComputeHmacSHA256(string message, string secretKey)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            using var hmac = new HMACSHA256(keyBytes);
            byte[] hash = hmac.ComputeHash(messageBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower(); // giống hmacHex bên Java
        }

        public async Task<bool> CancelPaymentOrder(long Ordercode)
        {
            var cancelpayment = await _databaseContext.PaymentOrder.FirstOrDefaultAsync(p => p.OrderCode == Ordercode);
            if(cancelpayment != null)
            {
                cancelpayment.Status = "Đã Hủy";
                await _databaseContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<CreatePaymentResult> CreatePaymentLink(int type, string UserName)
        {
            long orderCode = GenerateOrderCode();
            switch (type)
            {
                case 0:
                    ItemData item = new ItemData("Nâng cấp vip 3 ngày", quantity: 1, price: 45000);
                    List<ItemData> items = new List<ItemData>();
                    items.Add(item);

                    PaymentData paymentData = new PaymentData(orderCode, 45000, "VIP3",
                         items, cancelUrl: _configuration["PayOS:Cancel_url"]!, returnUrl: _configuration["PayOS:Return_url"]!);
                    CreatePaymentResult createPayment = await _payos.createPaymentLink(paymentData);
                    var PaymentOrder = new PaymentOrder
                    {
                        IdPaymentOrder = Guid.NewGuid().ToString(),
                        UserName = UserName,
                        OrderCode = orderCode,
                        TransactionId = createPayment.paymentLinkId,
                        Item = item.name,
                        Status = "Đang xử lý",                       
                    };
                    await _databaseContext.PaymentOrder.AddAsync(PaymentOrder);
                    await _databaseContext.SaveChangesAsync();
                    return createPayment;                   
                    
                case 1:
                    ItemData item2 = new ItemData("Nâng cấp vip 7 ngày", quantity: 1, price: 80000);
                    List<ItemData> items2 = new List<ItemData>();
                    items2.Add(item2);

                    PaymentData paymentData2 = new PaymentData(orderCode, 80000, "VIP7",
                         items2, cancelUrl: _configuration["PayOS:Cancel_url"]!, returnUrl: _configuration["PayOS:Return_url"]!);
                    CreatePaymentResult createPayment2 = await _payos.createPaymentLink(paymentData2);
                    var PaymentOrder2 = new PaymentOrder
                    {
                        IdPaymentOrder = Guid.NewGuid().ToString(),
                        UserName = UserName,
                        OrderCode = orderCode,
                        TransactionId = createPayment2.paymentLinkId,
                        Item = item2.name,
                        Status = "Đang xử lý",
                    };
                    await _databaseContext.PaymentOrder.AddAsync(PaymentOrder2);
                    await _databaseContext.SaveChangesAsync();
                    return createPayment2;
                case 2:
                    ItemData item3 = new ItemData("Nâng cấp vip 30 ngày", quantity: 1, price: 280000);
                    List<ItemData> items3 = new List<ItemData>();
                    items3.Add(item3);

                    PaymentData paymentData3 = new PaymentData(orderCode, 280000, "VIP30",
                         items3, cancelUrl: _configuration["PayOS:Cancel_url"]!, returnUrl: _configuration["PayOS:Return_url"]!);
                    CreatePaymentResult createPayment3 = await _payos.createPaymentLink(paymentData3);
                    var PaymentOrder3 = new PaymentOrder
                    {
                        IdPaymentOrder = Guid.NewGuid().ToString(),
                        UserName = UserName,
                        OrderCode = orderCode,
                        TransactionId = createPayment3.paymentLinkId,
                        Item = item3.name,
                        Status = "Đang xử lý",
                    };
                    await _databaseContext.PaymentOrder.AddAsync(PaymentOrder3);
                    await _databaseContext.SaveChangesAsync();
                    return createPayment3;
                default:
                    return null;
            }
        }

        public async Task<PaymentLinkInformation> GetDetailPaymentOrders(string UserName, long? OrderCode)
        {
            if (string.IsNullOrWhiteSpace(UserName)) throw new ArgumentNullException(nameof(UserName));
            var payment = await _databaseContext.PaymentOrder.FirstOrDefaultAsync(i => i.UserName == UserName && i.OrderCode == OrderCode);
            if (payment != null)
            {
                PaymentLinkInformation paymentLinkInformation = await _payos.getPaymentLinkInformation(payment.OrderCode);
                return paymentLinkInformation;
            }
            return null;
        }

        public async Task<List<PaymentOrder>> GetPaymentOrders(string UserName)
        {
            return await _databaseContext.PaymentOrder.Where(i => i.UserName == UserName).ToListAsync();
        }

        public async Task<bool> HandleWebhook(string requestBody, string receivedSignature)
        {
            string checksumKey = _configuration["PayOS:Checksum_Key"]!;

            try
            {
                // 1. Parse request body
                var payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestBody);
                if (payload == null || !payload.ContainsKey("data"))
                {
                    _logger.LogWarning("Webhook không chứa data");
                    return false;
                }

                // 2. Parse "data" thành JObject để xử lý key-value
                var dataJson = JObject.Parse(payload["data"]!.ToString()!);

                // 3. Tạo chuỗi key=value&key=value (sort theo alphabet)
                var sortedKeys = dataJson.Properties()
                                         .Select(p => p.Name)
                                         .OrderBy(name => name, StringComparer.Ordinal);

                var keyValuePairs = sortedKeys.Select(key =>
                {
                    var value = dataJson[key]?.ToString() ?? "";
                    return $"{key}={value}";
                });

                string dataToSign = string.Join("&", keyValuePairs);

                // 4. Tính chữ ký
                string calculatedSignature = ComputeHmacSHA256(dataToSign, checksumKey);

                // ✅ Nếu hợp lệ → xử lý logic
                var dataDict = dataJson.ToObject<Dictionary<string, object>>();
                if (payload["success"]?.ToString() == "True" &&
                    dataDict!["code"]?.ToString() == "00" &&
                    dataDict!["desc"]?.ToString() == "success")
                {
                    
                    if (long.TryParse(dataDict["orderCode"]?.ToString(), out long orderCodeLong))
                    {
                        var paymentOrder = await _databaseContext.PaymentOrder
                            .FirstOrDefaultAsync(i => i.OrderCode == orderCodeLong);

                        if (paymentOrder != null)
                        {
                            Match match = Regex.Match(paymentOrder.Item, @"\d+");
                            if (match.Success)
                            {
                                paymentOrder.Status = "Thành công";
                                int day = int.Parse(match.Value);
                                var user = await _userManager.Users.FirstOrDefaultAsync(i => i.UserName == paymentOrder.UserName);
                                var now = DateTimeHelper.GetDateTimeVnNowWithDateTimeUTC();                              
                                //Nạp lần đầu hoặc lâu mới nạp
                                if (user.TimeTopUp == null || user.ExpirationTime < now)
                                {
                                    user.TimeTopUp = DateTimeHelper.GetDateTimeVnNowWithDateTimeUTC();
                                    user.ExpirationTime = user.TimeTopUp.Value.AddDays(day);
                                    
                                }else //Nạp duy trì
                                {
                                    user.TimeTopUp = DateTimeHelper.GetDateTimeVnNowWithDateTimeUTC();
                                    user.ExpirationTime = user.ExpirationTime.Value.AddDays(day);
                                }
                                user.IsVip = true;
                                await _databaseContext.SaveChangesAsync();
                            }
                            
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xử lý webhook PayOS");
                return false;
            }
        }


    }

}

